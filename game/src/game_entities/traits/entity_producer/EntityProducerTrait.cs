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
      if (Entity.TryGetOwner(out var owner)) {
        owner.GetPlayerState().GetResourceType(data.CostType).Value -= data.Cost;
      }
    }
  }

  public void OnTurnStarted() {
    if (ProductionRoundsLeft > 0) {
      ProductionRoundsLeft--;
    }

    if (_currentProducedEntity != null) {
      if (Entity.TryGetOwner(out var owner)) {
        if (Entity.GameBoard.TryAddEntityOnCell(_currentProducedEntity.Entity, Entity.Cell, owner)) {
          _currentProducedEntity = null;
        }
      }
    }
  }
}
