namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class AttackTrait : Trait {
  [Export] public int Damage { get; private set; } = 1;
  [Export] public uint Range { get; private set; } = 1;

  public void Attack(Entity entity) {
    Entity.LookAt(entity.GlobalPosition, Vector3.Up);

    // animations

    if (entity.TryGetTrait<HealthTrait>(out var healthTrait)) {
      healthTrait.Damage(Damage);
    }
  }
}
