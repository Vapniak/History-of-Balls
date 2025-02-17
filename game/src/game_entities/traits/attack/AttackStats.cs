namespace HOB.GameEntity;
using Godot;
using System;

[GlobalClass]
public partial class AttackStats : BaseStat {
  [Export] public uint Damage { get; private set; }
  [Export] public uint Range { get; private set; }
}
