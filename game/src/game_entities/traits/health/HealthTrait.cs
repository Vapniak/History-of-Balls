namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class HealthTrait : Trait {
  [Signal] public delegate void DiedEventHandler();

  [Export] public uint StartHealth { get; set; } = 1;

  public uint CurrentHealth { get; private set; }

  public override void _Ready() {
    base._Ready();
    CurrentHealth = StartHealth;
  }

  public void Damage(uint damage) {
    if (damage > 0) {
      CurrentHealth -= damage;

      if (CurrentHealth <= 0) {
        Entity.QueueFree();
        EmitSignal(SignalName.Died);
      }
    }
  }
}
