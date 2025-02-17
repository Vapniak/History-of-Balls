namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class HealthTrait : Trait {
  [Signal] public delegate void DiedEventHandler();

  public override void _Ready() {
    GetStat<HealthStats>().CurrentHealth = (int)GetStat<HealthStats>().Health;
  }

  public void Damage(uint damage) {
    if (damage > 0) {
      GetStat<HealthStats>().CurrentHealth -= (int)damage;

      if (GetStat<HealthStats>().CurrentHealth <= 0) {
        Entity.QueueFree();
        EmitSignal(SignalName.Died);
      }
    }
  }
}
