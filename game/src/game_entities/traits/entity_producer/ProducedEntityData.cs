namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ProducedEntityData : Resource
{
  [Export] public EntityData Entity { get; private set; }
  [Export] public IncomeType CostType { get; private set; }
  [Export] public uint Cost { get; private set; }
  [Export] public uint RoundsProductionTime { get; private set; }
}
