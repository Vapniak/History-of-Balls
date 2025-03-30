namespace HOB;

using System;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass]
public partial class MatchPlayerSpawnData : Resource {
  [Export] public PlayerType PlayerType { get; private set; }
  [Export] public Country? Country { get; private set; }
  [Export] public int PrimaryResourceInitialValue { get; private set; }
  [Export] public int SecondaryResourceInitialValue { get; private set; }
  [Export] public Array<ProductionConfig>? ProducableEntities { get; private set; }
  [Export] public Array<EntitySpawnData>? Entities { get; private set; }
}

public enum PlayerType {
  Player,
  AI,
  None
}
