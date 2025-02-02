namespace HOB.GameEntity;

using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class AttackCommand : Command {
  public Entity[] AttackableEntities { get; set; }
  public bool Attacked { get; private set; }

  public bool TryAttack(Entity entity) {
    if (AttackableEntities.Contains(entity)) {
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
  public override bool IsAvailable() => !Attacked;
}
