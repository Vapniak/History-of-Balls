namespace HOB.GameEntity;
using Godot;
using System;

[GlobalClass]
public partial class AttackStats : BaseStat {
  public override string Name => "Attack";

  [Export] public uint Damage { get; private set; }
  [Export] public uint Range { get; private set; }
}
