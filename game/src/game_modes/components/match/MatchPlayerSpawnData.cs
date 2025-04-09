namespace HOB;

using GameplayTags;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass, Tool]
public partial class MatchPlayerSpawnData : Resource {
  [Export] public PlayerType PlayerType { get; private set; }
  [Export] public AIProfile? AIProfile { get; private set; }
  [Export] public Country? Country { get; private set; }
  [Export] public int PrimaryResourceInitialValue { get; private set; }
  [Export] public int SecondaryResourceInitialValue { get; private set; }
  [Export] public Array<ProductionConfig>? ProducableEntities { get; private set; }
  [Export] public Array<EntitySpawnData>? SpawnedEntities { get; private set; }
  [Export] public Array<EntityData> Entities { get; private set; } = new();
}

public enum PlayerType {
  Player,
  AI,
  None
}
