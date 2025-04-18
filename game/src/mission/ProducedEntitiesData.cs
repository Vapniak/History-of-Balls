namespace HOB;

using System;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class ProducedEntitiesData : Resource {
  [Export] public Array<ProductionConfig> ProducableEntities { get; private set; } = default!;
}