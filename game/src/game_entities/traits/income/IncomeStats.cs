namespace HOB.GameEntity;

using Godot;
using System;

public enum IncomeType {
  Primary,
  Secondary
}

[GlobalClass]
public partial class IncomeStats : BaseStat {
  [Export] public IncomeType IncomeType { get; private set; }
  [Export] public uint Value { get; private set; }
}
