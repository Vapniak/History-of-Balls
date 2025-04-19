namespace GameplayAbilitySystem;

using GameplayTags;
using Godot;

public partial class GameplayAbility {
  public abstract partial class Instance : Node {
    [Signal] public delegate void ActivatedEventHandler();
    [Signal] public delegate void EndedEventHandler();

    public GameplayAbility AbilityResource { get; private set; }
    public GameplayAbilitySystem OwnerAbilitySystem { get; private set; }
    protected GameplayEventData? CurrentEventData { get; private set; }

    public float Level { get; set; }
    public bool IsActive { get; private set; }

    protected GameplayEffectInstance? CooldownEffectInstance { get; private set; }
    protected GameplayEffectInstance? CostEffectInstance { get; private set; }

    public Instance(GameplayAbility abilityResource, GameplayAbilitySystem abilitySystem) {
      AbilityResource = abilityResource;
      OwnerAbilitySystem = abilitySystem;
    }

    public override void _Ready() {
      base._Ready();

      if (AbilityResource.ActivateOnGranted) {
        _ = OwnerAbilitySystem.TryActivateAbility(this, null);
      }
    }

    public virtual bool CanActivateAbility(GameplayEventData? eventData) {
      return !IsActive && CheckCost() && CheckCooldown() && CheckTags();
    }

    public virtual void CancelAbility() { EndAbility(true); }
    public virtual void PreActivate(GameplayEventData? eventData) {
      IsActive = true;
      CurrentEventData = eventData;
      EmitSignal(SignalName.Activated);
    }
    public virtual void ActivateAbility(GameplayEventData? eventData) { }
    public virtual void EndAbility(bool wasCanceled = false) {
      IsActive = false;
      CurrentEventData = null;
      EmitSignal(SignalName.Ended);
    }


    public virtual bool CheckCost() {
      if (AbilityResource.CostGameplayEffect == null) {
        return true;
      }

      return CheckCostFor(AbilityResource.CostGameplayEffect);
    }

    public virtual bool CheckCooldown() {
      if (AbilityResource.CooldownGameplayEffect?.GrantedTags != null) {
        return !OwnerAbilitySystem.OwnedTags.HasAllTags(AbilityResource.CooldownGameplayEffect.GrantedTags);
      }
      return true;
    }

    protected virtual bool CommitAbility() {
      return CommitCooldown() && CommitCost();
    }

    protected virtual bool CommitCooldown() {
      if (AbilityResource.CooldownGameplayEffect != null) {
        var instance = OwnerAbilitySystem.MakeOutgoingInstance(AbilityResource.CooldownGameplayEffect, 0);
        CooldownEffectInstance = instance;
        OwnerAbilitySystem.ApplyGameplayEffectToSelf(instance);
        return true;
      }

      return false;
    }

    protected virtual bool CommitCost() {
      if (AbilityResource.CostGameplayEffect != null) {
        var instance = OwnerAbilitySystem.MakeOutgoingInstance(AbilityResource.CostGameplayEffect, 0);
        CostEffectInstance = instance;
        OwnerAbilitySystem.ApplyGameplayEffectToSelf(instance);
        return true;
      }

      return false;
    }

    protected bool CheckCostFor(GameplayEffectResource effectResource, GameplayAbilitySystem? source = null, GameplayAbilitySystem? target = null) {
      source ??= OwnerAbilitySystem;
      target ??= source;

      if (effectResource != null) {
        if (effectResource?.EffectDefinition?.DurationPolicy != DurationPolicy.Instant) {
          return true;
        }

        if (effectResource.EffectDefinition.AttributeModifiers == null) {
          return true;
        }

        var ei = source.MakeOutgoingInstance(effectResource, Level, target);

        var aggregators = ei.GetAggregators();

        while (aggregators.MoveNext()) {
          var currentValue = target.AttributeSystem.GetAttributeCurrentValue(aggregators.Current.attribute).GetValueOrDefault();
          var costValue = aggregators.Current.aggregator.Evaluate(currentValue);

          if (costValue < GetCost(aggregators.Current.attribute)) {
            ei.QueueFree();
            return false;
          }
        }

        ei.QueueFree();
      }

      return true;
    }

    protected virtual float GetCost(GameplayAttribute attribute) {
      return 0;
    }

    protected void ExecuteGameplayCue(Tag cueTag, GameplayCueParameters parameters) {
      OwnerAbilitySystem.ExecuteGameplayCue(cueTag, parameters);
    }

    protected void AddGameplayCue(Tag cueTag, GameplayCueParameters parameters) {
      OwnerAbilitySystem.AddGameplayCue(cueTag, parameters);
    }

    protected void RemoveGameplayCue(Tag cueTag, GameplayCueParameters parameters) {
      OwnerAbilitySystem.RemoveGameplayCue(cueTag, parameters);
    }

    private bool CheckTags() {
      return
      (
        AbilityResource.ActivationBlockedTags == null ||
        !OwnerAbilitySystem.OwnedTags.HasAnyOfTags(AbilityResource.ActivationBlockedTags)
        ) &&
      (
        AbilityResource.ActivationRequiredTags == null ||
        OwnerAbilitySystem.OwnedTags.HasAllTags(AbilityResource.ActivationRequiredTags)
      );
    }
  }
}