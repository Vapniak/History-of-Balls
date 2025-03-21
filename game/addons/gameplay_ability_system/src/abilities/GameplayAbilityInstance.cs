namespace GameplayAbilitySystem;

using System.Threading.Tasks;
using Godot;

public abstract partial class GameplayAbilityInstance : Node {
  public GameplayAbilityResource AbilityResource { get; private set; }

  protected GameplayAbilitySystem OwnerAbilitySystem { get; private set; }

  protected GameplayEventData? CurrentEventData { get; private set; }

  public float Level { get; set; }
  public bool IsActive { get; set; }

  public GameplayAbilityInstance(GameplayAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) {
    AbilityResource = abilityResource;
    OwnerAbilitySystem = abilitySystem;
  }

  public virtual async Task<bool> TryActivateAbility(GameplayEventData eventData) {
    if (!CanActivateAbility(eventData)) {
      return false;
    }

    CurrentEventData = eventData;

    await PreActivate(eventData);
    await ActivateAbility(eventData);
    await EndAbility(eventData);

    CurrentEventData = null;

    return true;
  }

  public virtual bool CanActivateAbility(GameplayEventData eventData) {
    return !IsActive && CheckCost();
  }

  public virtual void CancelAbility() { }
  public virtual async Task PreActivate(GameplayEventData eventData) { IsActive = true; }
  public virtual async Task ActivateAbility(GameplayEventData eventData) { }
  public virtual async Task EndAbility(GameplayEventData eventData) { IsActive = false; }


  public virtual bool CheckCost() {
    return true;
  }

  public virtual bool CheckCooldown() {
    return true;
  }

  protected virtual bool CommitAbility() {
    return CommitCooldown() && CommitCost();
  }

  protected virtual bool CommitCooldown() {
    var instance = OwnerAbilitySystem.MakeOutgoingInstance(AbilityResource.CooldownGameplayEffect, 0);
    return OwnerAbilitySystem.TryApplyGameplayEffectToSelf(instance);
  }

  protected virtual bool CommitCost() {
    var instance = OwnerAbilitySystem.MakeOutgoingInstance(AbilityResource.CostGameplayEffect, 0);
    return OwnerAbilitySystem.TryApplyGameplayEffectToSelf(instance);
  }
}