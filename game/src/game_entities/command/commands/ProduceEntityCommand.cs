namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ProduceEntityCommand : Command {
  [Export] public EntityProducerTrait ProducerTrait { get; private set; }

  public bool TryStartProduceEntity(IMatchController caller, ProducedEntityData data) {
    if (GetEntity().TryGetStat<EntityProducerStats>(out var stats)) {
      if (!stats.ProducedEntities.Contains(data)) {
        return false;
      }

      if (CanBeUsed(caller) && CanEntityBeProduced(data)) {
        ProducerTrait.CurrentProducedEntity = data;
        Use();
        return true;
      }
    }

    return false;
  }

  public override bool CanBeUsed(IMatchController caller) => base.CanBeUsed(caller) && ProducerTrait.ProductionRoundsLeft == 0;

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

    Finish();
  }
}
