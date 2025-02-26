namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ProduceEntityCommand : Command {
  [Export] public EntityProducerTrait ProducerTrait { get; private set; }

  public bool TryProduceEntity(int entityIndex) {
    if (GetEntity().TryGetStat<EntityProducerStats>(out var stats)) {
      var data = stats.ProducedEntities[entityIndex];
      if (IsAvailable() && CanEntityBeProduced(data)) {
        Use();
        ProducerTrait.StartProduce(data);
        Finish();
        return true;
      }
    }

    return false;
  }

  public override bool IsAvailable() => base.IsAvailable() && ProducerTrait.ProductionRoundsLeft == 0;

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
