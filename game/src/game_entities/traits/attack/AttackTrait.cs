namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class AttackTrait : Trait {
  [Signal] public delegate void AttackFinishedEventHandler();

  private List<Entity> AttackableEntities { get; set; }

  public async Task Attack(Entity entity) {
    await Entity.TurnAt(entity.Cell.GetRealPosition(), 0.1f);

    if (entity.TryGetTrait<HealthTrait>(out var healthTrait)) {
      healthTrait.Damage(GetStat<AttackStats>().Damage);
    }

    EmitSignal(SignalName.AttackFinished);
  }

  public (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities() {
    AttackableEntities = new List<Entity>();
    var cellsInR = new List<GameCell>();

    var cells = Entity.Cell.GetCellsInRange(GetStat<AttackStats>().Range);
    foreach (var cell in cells) {
      cellsInR.Add(cell);
      var entities = Entity.GameBoard.GetEntitiesOnCell(cell);
      AttackableEntities.AddRange(entities.Where(e => !e.IsOwnedBy(Entity.OwnerController) && e.TryGetTrait<HealthTrait>(out _)));
    }


    return (AttackableEntities.ToArray(), cellsInR.ToArray());
  }
}
