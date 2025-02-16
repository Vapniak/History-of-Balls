namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class HealthStats : BaseStat {
  public override string Name => "Health";

  [Export] public uint Health { get; private set; } = 0;
}
