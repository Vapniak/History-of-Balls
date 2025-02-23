namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class FactoryStats : BaseStat {
  [Export] public uint ProcessingTurns { get; private set; } = 1;
  [Export] public uint InputValue { get; private set; } = 2;
  [Export] public IncomeType InputType { get; private set; } = IncomeType.Secondary;
  [Export] public uint OutputValue { get; private set; } = 1;
  [Export] public IncomeType OutputType { get; private set; } = IncomeType.Primary;
}
