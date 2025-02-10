namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class MovementStats : BaseStat {
  public override string Name => "Movement";
  [Export] public uint MovePoints { get; private set; }
  [Export] public float MoveSpeed { get; private set; }
}
