namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class EntityDefinition : Resource {
  [Export] public string Name { get; private set; }
  [Export] public PackedScene EntityScene { get; private set; }
}
