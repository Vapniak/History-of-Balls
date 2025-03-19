namespace HOB.GameEntity;

using GameplayAbilitySystem;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class EntityData : Resource {
  [Export] public string EntityName { get; private set; }
  [Export] public Array<GameplayAttributeSet> Attributes { get; private set; }
  [Export] public Array<GameplayAbility> Abilities { get; private set; }
  [Export] public PackedScene Body { get; private set; }

  public Entity CreateEntity(GameCell cell, IEntityManagment entityManagment) {
    var body = Body.Instantiate<EntityBody>();

    return new(EntityName, cell, entityManagment, Attributes, Abilities, body);
  }
}
