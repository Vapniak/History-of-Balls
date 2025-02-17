namespace HOB.GameEntity;

using Godot;
using System;
using System.Collections.Generic;
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
      if (entities.Length > 0 && !entities[0].IsOwnedBy(Entity.OwnerController) && entities[0].TryGetTrait<HealthTrait>(out _)) {
        GD.Print(entities[0].GetEntityName());
        AttackableEntities.Add(entities[0]);
      }
    }


    return (AttackableEntities.ToArray(), cellsInR.ToArray());
  }
}
