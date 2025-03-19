namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public abstract partial class GameplayAbility : Resource {
  [Export] public GameplayEffect CooldownGameplayEffect { get; private set; }
  [Export] public GameplayEffect CostGameplayEffect { get; private set; }


  public virtual void ApplyCooldown(GameplayAbilityInstance abilityInstance) {

  }

  public virtual void ApplyCost(GameplayAbilityInstance abilityInstance) {

  }

  public virtual bool ShouldAbilityRespondToEvent(GameplayEventData payload) {
    return false;
  }

  public virtual bool CanActivateAbility(GameplayAbilityInstance abilityInstance, GameplayEventData payload) {
    return CheckCooldown(abilityInstance) && CheckCost(abilityInstance);
  }

  public virtual bool CanBeCanceled(GameplayAbilityInstance abilityInstance) {
    return true;
  }

  public virtual void CancelAbility(GameplayAbilityInstance abilityInstance) {

  }

  public virtual bool CheckCooldown(GameplayAbilityInstance abilityInstance) {
    return true;
  }

  public virtual bool CheckCost(GameplayAbilityInstance abilityInstance) {
    return true;
  }

  public virtual bool CommitAbility(GameplayAbilityInstance abilityInstance) {
    CommitAbilityCooldown(abilityInstance);
    CommitAbilityCost(abilityInstance);
    return true;
  }

  public virtual bool CommitAbilityCooldown(GameplayAbilityInstance abilityInstance) {
    ApplyCooldown(abilityInstance);
    return true;
  }

  public virtual bool CommitAbilityCost(GameplayAbilityInstance abilityInstance) {
    ApplyCost(abilityInstance);
    return true;
  }

  public bool TryActivateAbility(GameplayAbilityInstance abilityInstance, GameplayEventData payload) {
    if (CanActivateAbility(abilityInstance, payload)) {
      ActivateAbility(abilityInstance, payload);
      return true;
    }

    return false;
  }


  protected virtual void ActivateAbility(GameplayAbilityInstance abilityInstance, GameplayEventData triggerEventData) {
    // commit ability
  }

  protected virtual void EndAbility(GameplayAbilityInstance abilityInstance) {

  }
}
