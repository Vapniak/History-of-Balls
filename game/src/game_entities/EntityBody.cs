namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class EntityBody : Area3D {
  public override void _Ready() {
    CollisionLayer = GameLayers.Physics3D.Mask.Entity;
    CollisionMask = 0;
  }
}
