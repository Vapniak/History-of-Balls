namespace HOB.GameEntity;

using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class ProduceEntityCommand : Command {
  [Export] public EntityProducerTrait ProducerTrait { get; private set; }

  public bool TryStartProduceEntity(ProducedEntityData data) {
    if (GetEntity().TryGetStat<EntityProducerStats>(out var stats)) {
      if (!stats.ProducedEntities.Contains(data)) {
        return false;
      }

      if (CanBeUsed() && CanEntityBeProduced(data)) {
        ProducerTrait.CurrentProducedEntity = data;
        Use();
        return true;
      }
    }

    return false;
  }

  public override bool CanBeUsed() => base.CanBeUsed() && ProducerTrait.ProductionRoundsLeft == 0 && GetEntity().TryGetStat<EntityProducerStats>(out var stat) && stat.ProducedEntities.Any(CanEntityBeProduced);

  public override void OnTurnStarted() {
    base.OnTurnStarted();

    if (!IsOwnerCurrentTurn()) {
      return;
    }

    if (ProducerTrait.ProductionRoundsLeft > 0) {
      ProducerTrait.ProductionRoundsLeft--;
    }

    if (ProducerTrait.ProductionRoundsLeft == 0 && ProducerTrait.CurrentProducedEntity != null) {
      if (ProducerTrait.TryProduce(ProducerTrait.CurrentProducedEntity)) {
        ProducerTrait.CurrentProducedEntity = null;
        Finish();
      }
    }
  }

  public bool CanEntityBeProduced(ProducedEntityData data) {
    if (GetEntity().TryGetOwner(out var owner)) {
      if (owner.GetPlayerState().GetResourceType(data.CostType).Value >= data.Cost) {
        return true;
      }
    }

    return false;
  }

  protected override void Use() {
    base.Use();

    if (GetEntity().TryGetOwner(out var owner)) {
      owner.GetPlayerState().GetResourceType(ProducerTrait.CurrentProducedEntity.CostType).Value -= ProducerTrait.CurrentProducedEntity.Cost;
      ProducerTrait.ProductionRoundsLeft = ProducerTrait.CurrentProducedEntity.RoundsProductionTime;
    }
  }
}
