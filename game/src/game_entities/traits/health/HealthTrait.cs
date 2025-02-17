namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class HealthTrait : Trait {
  [Signal] public delegate void DiedEventHandler();

  public int CurrentHealth { get; private set; }

  public override void _Ready() {
    base._Ready();
    CurrentHealth = (int)GetStat<HealthStats>().Health;
  }

  public void Damage(uint damage) {
    if (damage > 0) {
      CurrentHealth -= (int)damage;

      if (CurrentHealth <= 0) {
        Entity.QueueFree();
        EmitSignal(SignalName.Died);
      }
    }
  }
}
