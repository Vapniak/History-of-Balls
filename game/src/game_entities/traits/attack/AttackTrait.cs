namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class AttackTrait : Trait {
  [Signal] public delegate void AttackFinishedEventHandler();

  private List<Entity> AttackableEntities { get; set; }
  public bool TryAttack(Entity entity) {
    if (!AttackableEntities.Contains(entity)) {
      return false;
    }

    Entity.LookAt(entity.GetPosition());

    // animations
    // FIXME: FINISH IS BEFORE START
    EmitSignal(SignalName.AttackFinished);

    if (entity.TryGetTrait<HealthTrait>(out var healthTrait)) {
      healthTrait.Damage(GetStat<AttackStats>().Damage);
    }

    return true;
  }

  public (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities(GameBoard board) {
    AttackableEntities = new List<Entity>();
    var cellsInR = new List<GameCell>();

    var cells = board.GetCellsInRange(Entity.Cell.Coord, GetStat<AttackStats>().Range);
    foreach (var cell in cells) {
      cellsInR.Add(cell);
      var entities = board.GetEntitiesOnCell(cell);
      if (entities.Length > 0 && !entities[0].IsOwnedBy(Entity.OwnerController)) {
        AttackableEntities.Add(entities[0]);
      }
    }


    return (AttackableEntities.ToArray(), cellsInR.ToArray());
  }
}
