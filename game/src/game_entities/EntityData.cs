namespace HOB.GameEntity;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class EntityData : Resource {
  [Export] public string EntityName { get; private set; } = "Entity";
  [Export] public AttributeSetsContainer? AttributeSetsContainer { get; private set; }
  [Export] public Array<GameplayAbilityResource>? Abilities { get; private set; }
  [Export] public TagContainer? Tags { get; private set; }
  [Export] public PackedScene? Body { get; private set; }

  public Entity? CreateEntity(GameCell cell, IEntityManagment entityManagment, IMatchController? owner) {
    var body = Body?.Instantiate<EntityBody>();

    if (body != null) {
      return new(EntityName, cell, entityManagment, AttributeSetsContainer, Abilities, Tags, body, owner);
    }

    return null;
  }
}
