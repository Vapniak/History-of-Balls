namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public abstract partial class HOBEntityAbilityInstance : HOBAbilityInstance {
  protected Entity OwnerEntity => OwnerAbilitySystem.GetOwner<Entity>();
  protected HOBEntityAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
    AbilityResource.ActivationBlockedTags?.AddTag(TagManager.GetTag(HOBTags.StateDead));
  }

  public override bool CanActivateAbility(GameplayEventData? eventData) {
    if (eventData != null) {
      return base.CanActivateAbility(eventData) && OwnerEntity.TryGetOwner(out var owner) && owner == eventData.Activator;
    }

    // TODO: fix that can activate always
    return base.CanActivateAbility(eventData);
  }
}
