namespace HOB;

using System;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass]
public partial class EntitySpawnData : Resource {
  [Export] public EntityData? EntityData { get; private set; }
  [Export] public Array<Vector2I>? SpawnAt { get; private set; }
}
