namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;

public partial class HOBAbility {
  public abstract partial class EntityInstance : Instance {
    public Entity OwnerEntity => OwnerAbilitySystem.GetOwner<Entity>();
    protected EntityInstance(HOBAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
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
}
