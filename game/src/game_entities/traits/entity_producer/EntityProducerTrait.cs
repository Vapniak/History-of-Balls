namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class EntityProducerTrait : Trait {
  public ProducedEntityData CurrentProducedEntity;
  [Notify]
  public uint ProductionRoundsLeft { get => _productionRoundsLeft.Get(); set => _productionRoundsLeft.Set(value); }
  public bool TryProduce(ProducedEntityData data) {
    if (Entity.TryGetOwner(out var owner)) {
      if (Entity.EntityManagment.TryAddEntityOnCell(data.Entity, Entity.Cell, owner)) {
        return true;
      }
    }
    return false;
  }
}
