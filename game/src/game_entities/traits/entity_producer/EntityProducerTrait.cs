namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class EntityProducerTrait : Trait {
  public uint ProductionRoundsLeft;

  public ProducedEntityData CurrentProducedEntity;
  public void StartProduce(ProducedEntityData data) {
    if (Entity.TryGetStat<EntityProducerStats>(out var producerStats)) {
      CurrentProducedEntity = data;
      ProductionRoundsLeft = CurrentProducedEntity.RoundsProductionTime;
    }
  }

  protected override void OnOwnerChanged() {
    base.OnOwnerChanged();

    ProductionRoundsLeft = 0;
    CurrentProducedEntity = null;
  }

  public void OnTurnStarted() {
    if (ProductionRoundsLeft > 0) {
      ProductionRoundsLeft--;

      if (ProductionRoundsLeft == 0) {
        if (CurrentProducedEntity != null) {
          if (Entity.TryGetOwner(out var owner)) {
            if (Entity.EntityManagment.TryAddEntityOnCell(CurrentProducedEntity.Entity, Entity.Cell, owner)) {
              CurrentProducedEntity = null;
            }
          }
        }
      }
    }
  }
}
