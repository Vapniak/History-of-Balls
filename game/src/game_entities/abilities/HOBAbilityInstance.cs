namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public abstract partial class HOBAbilityInstance : GameplayAbilityInstance {
  public new HOBAbilityResource AbilityResource { get; private set; }
  public Entity OwnerEntity => OwnerAbilitySystem.GetOwner<Entity>();
  protected HOBAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
    AbilityResource = abilityResource;

    AbilityResource.ActivationBlockedTags?.AddTag(TagManager.GetTag(HOBTags.StateDead));
  }

  public override bool CanActivateAbility(GameplayEventData? eventData) {
    if (eventData != null) {
      return base.CanActivateAbility(eventData) && OwnerEntity.TryGetOwner(out var owner) && owner == eventData.Value.TargetData.Caller;
    }

    return false;
  }
}