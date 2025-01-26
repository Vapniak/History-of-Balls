namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class MoveTraitData : TraitData {
  [Export] public uint MovePoints { get; private set; }
}
