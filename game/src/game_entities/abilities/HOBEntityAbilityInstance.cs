namespace HOB;

using GameplayAbilitySystem;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public abstract partial class HOBEntityAbilityInstance : HOBAbilityInstance {
  protected Entity OwnerEntity => OwnerAbilitySystem.GetOwner<Entity>();
  protected HOBEntityAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
  }

  public override bool CanActivateAbility(GameplayEventData? eventData) {
    if (eventData != null) {
      return base.CanActivateAbility(eventData) && OwnerEntity.TryGetOwner(out var owner) && owner == eventData.Value.TargetData.Caller;
    }

    return base.CanActivateAbility(eventData);
  }
}
