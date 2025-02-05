namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class AttackTrait : Trait {
  [Signal] public delegate void AttackFinishedEventHandler();

  [Export] public int Damage { get; private set; } = 1;
  [Export] public uint Range { get; private set; } = 1;

  public void Attack(Entity entity) {
    Entity.LookAt(entity.GetPosition());

    // animations
    EmitSignal(SignalName.AttackFinished);

    if (entity.TryGetTrait<HealthTrait>(out var healthTrait)) {
      healthTrait.Damage(Damage);
    }
  }
}
