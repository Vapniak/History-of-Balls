namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class EntityProducerTrait : Trait {
  public uint ProductionRoundsLeft;

  private ProducedEntityData _currentProducedEntity;
  public void StartProduce(ProducedEntityData data) {
    if (Entity.TryGetStat<EntityProducerStats>(out var producerStats)) {
      _currentProducedEntity = data;
      ProductionRoundsLeft = _currentProducedEntity.RoundsProductionTime;
    }
  }

  protected override void OnOwnerChanged() {
    base.OnOwnerChanged();

    ProductionRoundsLeft = 0;
    _currentProducedEntity = null;
  }

  public void OnTurnStarted() {
    if (ProductionRoundsLeft > 0) {
      ProductionRoundsLeft--;

      if (ProductionRoundsLeft == 0) {
        if (_currentProducedEntity != null) {
          if (Entity.TryGetOwner(out var owner)) {
            if (Entity.GameBoard.TryAddEntityOnCell(_currentProducedEntity.Entity, Entity.Cell, owner)) {
              _currentProducedEntity = null;
            }
          }
        }
      }
    }
  }
}
