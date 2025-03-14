namespace HOB.GameEntity;

using GameplayFramework;
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

      var text = FloatingText.Create($"-{damage}", Colors.Red);
      GameInstance.GetWorld().AddChild(text);
      text.GlobalPosition = Entity.GetPosition() + Vector3.Up * 2;
      text.Animate();

      if (GetStat<HealthStats>().CurrentHealth <= 0) {
        Entity.QueueFree();
        EmitSignal(SignalName.Died);
      }
    }
  }
}
