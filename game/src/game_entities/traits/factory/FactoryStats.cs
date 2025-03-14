namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class FactoryStats : BaseStat {
  [Export] public uint ProcessingTurns { get; private set; } = 1;
  [Export] public uint ProcessedValue { get; private set; } = 2;
  [Export] public IncomeType ProcessedResource { get; private set; } = IncomeType.Secondary;
  [Export] public uint ProducedValue { get; private set; } = 1;
  [Export] public IncomeType ProducedResource { get; private set; } = IncomeType.Primary;
}
