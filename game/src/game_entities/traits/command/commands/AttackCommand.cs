namespace HOB.GameEntity;

using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class AttackCommand : Command {
  public Entity[] AttackableEntities { get; set; }
  public bool Attacked { get; private set; }

  public AttackTrait EntityAttackTrait { get; private set; }

  public override void _Ready() {
    base._Ready();

    EntityAttackTrait = GetEntity().GetTrait<AttackTrait>();
    EntityAttackTrait.AttackFinished += Finish;
  }

  public bool TryAttack(Entity entity) {
    if (AttackableEntities.Contains(entity)) {
      Start();
      GetEntity().GetTrait<AttackTrait>().Attack(entity);
      Attacked = true;
      return true;
    }

    return false;
  }

  public override void OnRoundChanged(int roundNumber) {
    base.OnRoundChanged(roundNumber);
    Attacked = false;
  }
  public override bool IsAvailable() => base.IsAvailable() && !Attacked;
}
