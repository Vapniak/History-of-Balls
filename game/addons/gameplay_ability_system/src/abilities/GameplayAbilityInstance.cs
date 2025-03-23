namespace GameplayAbilitySystem;

using System.Threading.Tasks;
using Godot;

public abstract partial class GameplayAbilityInstance : Node {
  [Signal] public delegate void ActivatedEventHandler();
  [Signal] public delegate void EndedEventHandler();

  public GameplayAbilityResource AbilityResource { get; private set; }
  protected GameplayAbilitySystem OwnerAbilitySystem { get; private set; }
  protected GameplayEventData? CurrentEventData { get; private set; }

  public float Level { get; set; }
  public bool IsActive { get; set; }

  public GameplayAbilityInstance(GameplayAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) {
    AbilityResource = abilityResource;
    OwnerAbilitySystem = abilitySystem;
  }

  public virtual async Task<bool> TryActivateAbility(GameplayEventData? eventData) {
    if (!CanActivateAbility(eventData)) {
      return false;
    }

    await PreActivate(eventData);
    await ActivateAbility(eventData);

    return true;
  }

  public virtual bool CanActivateAbility(GameplayEventData? eventData) {
    return !IsActive && CheckCost() && CheckCooldown() && CheckTags();
  }

  public virtual void CancelAbility() { }
  public virtual async Task PreActivate(GameplayEventData? eventData) {
    IsActive = true;
    CurrentEventData = eventData;
    EmitSignal(SignalName.Activated);
  }
  public virtual async Task ActivateAbility(GameplayEventData? eventData) { }
  public virtual async Task EndAbility(GameplayEventData? eventData) {
    IsActive = false;
    CurrentEventData = null;
    EmitSignal(SignalName.Ended);
  }


  public virtual bool CheckCost() {
    return true;
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
      return OwnerAbilitySystem.TryApplyGameplayEffectToSelf(instance);
    }

    return false;
  }

  protected virtual bool CommitCost() {
    if (AbilityResource.CostGameplayEffect != null) {
      var instance = OwnerAbilitySystem.MakeOutgoingInstance(AbilityResource.CostGameplayEffect, 0);
      return OwnerAbilitySystem.TryApplyGameplayEffectToSelf(instance);
    }

    return false;
  }

  private bool CheckTags() {
    if (AbilityResource.ActivationBlockedTags == null) {
      return true;
    }

    return !OwnerAbilitySystem.OwnedTags.HasAllTags(AbilityResource.ActivationBlockedTags);
  }
}