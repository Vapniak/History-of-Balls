namespace GameplayAbilitySystem;

using System.Threading.Tasks;
using Godot;

public abstract partial class GameplayAbilityInstance : Node {
  [Signal] public delegate void ActivatedEventHandler();
  [Signal] public delegate void EndedEventHandler();

  public GameplayAbilityResource AbilityResource { get; private set; }
  public GameplayAbilitySystem OwnerAbilitySystem { get; private set; }
  protected GameplayEventData? CurrentEventData { get; private set; }

  public float Level { get; set; }
  public bool IsActive { get; private set; }

  public GameplayAbilityInstance(GameplayAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) {
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

  public virtual void CancelAbility() { EndAbility(null); }
  public virtual void PreActivate(GameplayEventData? eventData) {
    IsActive = true;
    CurrentEventData = eventData;
    EmitSignal(SignalName.Activated);
  }
  public virtual void ActivateAbility(GameplayEventData? eventData) { }
  public virtual void EndAbility(GameplayEventData? eventData) {
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
      OwnerAbilitySystem.ApplyGameplayEffectToSelf(instance);
      return true;
    }

    return false;
  }

  protected virtual bool CommitCost() {
    if (AbilityResource.CostGameplayEffect != null) {
      var instance = OwnerAbilitySystem.MakeOutgoingInstance(AbilityResource.CostGameplayEffect, 0);
      OwnerAbilitySystem.ApplyGameplayEffectToSelf(instance);
      return true;
    }

    return false;
  }

  protected bool CheckCostFor(GameplayEffectResource effectResource, GameplayAbilitySystem? source = null, GameplayAbilitySystem? target = null) {
    source ??= OwnerAbilitySystem;
    target ??= source;

    if (effectResource != null) {
      var ei = source.MakeOutgoingInstance(effectResource, Level);

      if (ei.GameplayEffect?.EffectDefinition?.DurationPolicy != DurationPolicy.Instant) {
        return true;
      }

      if (ei.GameplayEffect.EffectDefinition.Modifiers == null) {
        return true;
      }

      foreach (var modifier in ei.GameplayEffect.EffectDefinition.Modifiers) {
        if (modifier.ModifierType != AttributeModifierType.Add) {
          continue;
        }

        var costValue = (modifier.ModifierMagnitude == null ? 1 : modifier.ModifierMagnitude.CalculateMagnitude(ei)) * modifier.Coefficient;

        if (modifier.Attribute == null) {
          continue;
        }

        var value = target.AttributeSystem.GetAttributeCurrentValue(modifier.Attribute);

        if (value + costValue < GetCost(modifier.Attribute)) {
          return false;
        }
      }
    }

    return true;
  }

  protected virtual float GetCost(GameplayAttribute attribute) {
    return 0;
  }

  private bool CheckTags() {
    return
    (
      AbilityResource.ActivationBlockedTags == null ||
      !OwnerAbilitySystem.OwnedTags.HasAllTags(AbilityResource.ActivationBlockedTags)
      ) &&
    (
      AbilityResource.ActivationRequiredTags == null ||
      OwnerAbilitySystem.OwnedTags.HasAllTags(AbilityResource.ActivationRequiredTags)
    );
  }
}
