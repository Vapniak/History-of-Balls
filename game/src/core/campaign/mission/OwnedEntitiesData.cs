namespace HOB;

using System;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass, Tool]
public partial class OwnedEntitiesData : Resource {
  [Export] public Array<EntityData> Entities { get; set; } = default!;
}