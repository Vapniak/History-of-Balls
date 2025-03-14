namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class MovementStats : BaseStat {
  [Export] public uint MovePoints { get; private set; }
}
