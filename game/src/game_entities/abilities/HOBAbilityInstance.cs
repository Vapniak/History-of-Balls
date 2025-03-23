namespace HOB;

using GameplayAbilitySystem;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public abstract partial class HOBAbilityInstance : GameplayAbilityInstance {
  public new HOBAbilityResource AbilityResource { get; private set; }
  public Entity OwnerEntity => OwnerAbilitySystem.GetOwner<Entity>();
  protected HOBAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {
    AbilityResource = abilityResource;
  }
}