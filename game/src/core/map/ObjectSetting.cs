namespace HOB;

using Godot;
using HOB.GameEntity;
using System;

[GlobalClass, Tool]
public partial class ObjectSetting : Resource {
  [Export] public string Name { get; set; } = "";
  [Export] public EntityData EntityData { get; private set; } = default!;
}
