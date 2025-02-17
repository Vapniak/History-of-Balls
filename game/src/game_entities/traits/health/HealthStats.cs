namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class HealthStats : BaseStat {
  [Export] public uint Health { get; private set; } = 0;
  public int CurrentHealth { get; set; }
}
