namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;

[GlobalClass]
public partial class MoveAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute MovePoints { get; private set; }
}
