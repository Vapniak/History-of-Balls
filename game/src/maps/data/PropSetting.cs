namespace HOB;

using System;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class PropSetting : Resource {
  [Export] public EntityData PropData { get; private set; } = default!;
  [Export(PropertyHint.Range, "0,100")] public float Chance { get; private set; }
}