namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class MoveTraitData : TraitData {
  [Export] public uint Move { get; private set; }
}
