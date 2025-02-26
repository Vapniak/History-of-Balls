namespace HOB.GameEntity;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class EntityProducerStats : BaseStat {
  [Export] public Array<ProducedEntityData> ProducedEntities { get; private set; }
}
