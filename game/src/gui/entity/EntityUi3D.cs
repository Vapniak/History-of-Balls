namespace HOB;

using GameplayTags;
using Godot;
using HOB;
using HOB.GameEntity;
using System;

public partial class EntityUi3D : Node3D {
  [Export] public EntityUI EntityUI { get; private set; }

  private static readonly string _ui3dUID = "uid://omhtmi8gorif";
  public static EntityUi3D Create(Entity owner) {
    var entityUI3D = ResourceLoader.Load<PackedScene>(_ui3dUID).Instantiate<EntityUi3D>();
    entityUI3D.EntityUI?.Initialize(owner);

    var aabb = new Aabb();
    foreach (var child in owner.Body.GetAllChildren()) {
      if (child is MeshInstance3D mesh) {
        aabb = aabb.Merge(mesh.GetAabb());
      }
    }

    entityUI3D.Position = aabb.GetCenter() + Vector3.Up * (aabb.Size.Y + 2);

    if (owner.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure))) {
      entityUI3D.Position += Vector3.Up * 2;
    }

    return entityUI3D;
  }
}
