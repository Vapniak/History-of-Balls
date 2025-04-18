namespace HOB;

using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class MatchPlayerSpawnData : Resource {
  [Export] public PlayerType PlayerType { get; private set; }
  [Export] public AIProfile? AIProfile { get; private set; }
  [Export] public Country Country { get; private set; } = default!;
  [Export] public ProducedEntitiesData ProducableEntities { get; private set; } = default!;
  [Export] public Array<EntitySpawnData>? SpawnedEntities { get; private set; }
  [Export] public OwnedEntitiesData OwnedEntities { get; private set; } = default!;
}

public enum PlayerType {
  Player,
  AI,
  None
}
