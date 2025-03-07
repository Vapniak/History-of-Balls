namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ProduceEntityCommand : Command {
  [Export] public EntityProducerTrait ProducerTrait { get; private set; }

  public bool TryProduceEntity(IMatchController caller, ProducedEntityData data) {
    if (GetEntity().TryGetStat<EntityProducerStats>(out var stats)) {
      if (!stats.ProducedEntities.Contains(data)) {
        return false;
      }

      if (CanBeUsed(caller) && CanEntityBeProduced(data)) {
        Use();
        caller.GetPlayerState().GetResourceType(data.CostType).Value -= data.Cost;
        ProducerTrait.StartProduce(data);
        Finish();
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

    ProducerTrait.OnTurnStarted();
  }

  public bool CanEntityBeProduced(ProducedEntityData data) {
    if (GetEntity().TryGetOwner(out var owner)) {
      if (owner.GetPlayerState().GetResourceType(data.CostType).Value >= data.Cost) {
        return true;
      }
    }

    return false;
  }
}
