namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class ProduceEntityCommand : Command {
  [Export] public EntityProducerTrait ProducerTrait { get; private set; }

  public bool TryProduceEntity(ProducedEntityData data) {
    if (GetEntity().TryGetOwner(out var owner)) {
      if (GetEntity().TryGetStat<EntityProducerStats>(out var stats)) {
        if (!stats.ProducedEntities.Contains(data)) {
          return false;
        }

        if (IsAvailable() && CanEntityBeProduced(data)) {
          Use();
          owner.GetPlayerState().GetResourceType(data.CostType).Value -= data.Cost;
          ProducerTrait.StartProduce(data);
          Finish();
          return true;
        }
      }
    }

    return false;
  }

  public override bool IsAvailable() => true;

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
