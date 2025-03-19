namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public abstract partial class GameplayAbility : Resource {
  [Export] public GameplayEffect CooldownGameplayEffect { get; private set; }
  [Export] public GameplayEffect CostGameplayEffect { get; private set; }


  public virtual void OnGranted(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {

  }

  public virtual void ApplyCooldown(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {

  }

  public virtual void ApplyCost(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {

  }

  public virtual bool ShouldAbilityRespondToEvent(GameplayAbilityOwnerInfo ownerInfo, GameplayEventData payload) {
    return false;
  }

  public virtual bool CanActivateAbility(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo, GameplayEventData payload) {
    return CheckCooldown(abilityInstance, ownerInfo) && CheckCost(abilityInstance, ownerInfo);
  }

  public virtual bool CanBeCanceled(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {
    return true;
  }

  public virtual void CancelAbility(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {

  }

  public virtual bool CheckCooldown(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {
    return true;
  }

  public virtual bool CheckCost(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {
    return true;
  }

  public virtual bool CommitAbility(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {
    if (!CommitAbilityCooldown(abilityInstance, ownerInfo) && CommitAbilityCost(abilityInstance, ownerInfo)) {
      return true;
    }
    return false;
  }

  public virtual bool CommitAbilityCooldown(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {
    ApplyCooldown(abilityInstance, ownerInfo);
    return true;
  }

  public virtual bool CommitAbilityCost(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {
    ApplyCost(abilityInstance, ownerInfo);
    return true;
  }
  public bool TryActivateAbility(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo, GameplayEventData payload) {
    if (CanActivateAbility(abilityInstance, ownerInfo, payload)) {
      PreActivate(abilityInstance, ownerInfo, payload);
      ActivateAbility(abilityInstance, ownerInfo, payload);
      return true;
    }

    return false;
  }


  protected virtual void PreActivate(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo, GameplayEventData payload) {
    abilityInstance.CurrentOwnerInfo = ownerInfo;
    abilityInstance.CurrentEventData = payload;
    abilityInstance.IsActive = true;
  }

  protected virtual void ActivateAbility(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo, GameplayEventData triggerEventData) {
    // commit ability
  }

  protected virtual void EndAbility(GameplayAbilityInstance abilityInstance, GameplayAbilityOwnerInfo ownerInfo) {
    abilityInstance.CurrentOwnerInfo = null;
    abilityInstance.CurrentEventData = null;
    abilityInstance.IsActive = false;
  }
}
