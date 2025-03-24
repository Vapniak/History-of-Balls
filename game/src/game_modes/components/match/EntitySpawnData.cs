namespace HOB;

using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class EntitySpawnData : Resource {
  [Export] public EntityData? EntityData { get; private set; }
  [Export] public int Col { get; private set; }
  [Export] public int Row { get; private set; }
}
