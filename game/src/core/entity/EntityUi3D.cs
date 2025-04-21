namespace HOB;

using GameplayTags;
using Godot;
using HOB;
using HOB.GameEntity;
using System;

public partial class EntityUi3D : Sprite3D {
  [Export] public EntityUIWidget EntityUI { get; private set; } = default!;

  private static readonly string _ui3dUID = "uid://omhtmi8gorif";

  private Entity Entity { get; set; } = default!;
  public static EntityUi3D Create(Entity owner) {
    var entityUI3D = ResourceLoader.Load<PackedScene>(_ui3dUID).Instantiate<EntityUi3D>();
    entityUI3D.EntityUI?.BindTo(owner);
    entityUI3D.Entity = owner;
    return entityUI3D;
  }

  public override void _Ready() {
    //var aabb = Entity.Body.GetCombinedAABB(true);
    //GD.Print(Entity.EntityName, aabb.Size);
    //Position = Vector3.Up * aabb.End.Y;
    //DebugDraw3D.DrawAabb(aabb, duration: 60);

    Position = Vector3.Up * 2f;


    if (Entity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure))) {
      Position += Vector3.Up * 2;
    }
  }
}
