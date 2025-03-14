namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class HealthStats : BaseStat {
  [Export] public uint Health { get; private set; } = 0;
  [Notify]
  public int CurrentHealth { get => _currentHealth.Get(); set => _currentHealth.Set(value); }
}
