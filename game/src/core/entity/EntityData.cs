namespace HOB.GameEntity;

using System.Diagnostics;
using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class EntityData : Resource {
  [Export] public string EntityName { get; private set; } = "Entity";
  [Export] public AttributeSetsContainer? AttributeSetsContainer { get; private set; }
  [Export] public Array<GameplayAbility>? Abilities { get; private set; }
  [Export] public TagContainer? Tags { get; private set; }
  [Export] public PackedScene? Body { get; private set; }

  public Texture2D? Icon => GameInstance.GetGameMode<HOBGameMode>()?.GetIconFor(this);

  public Entity? CreateEntity(GameCell cell, IEntityManagment entityManagment, IMatchController? owner) {
    var body = Body?.InstantiateOrNull<EntityBody>();
    if (body != null) {
      return new(EntityName, cell, entityManagment, AttributeSetsContainer, Abilities, Tags, body, owner);
    }
    else {
      Debug.Assert(false, "Body is null");
    }

    return null;
  }
}
