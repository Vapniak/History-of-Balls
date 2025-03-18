namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;

[GlobalClass]
public partial class AttackAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute Damage { get; private set; } = new();
  [Export] public GameplayAttribute Range { get; private set; } = new();
}
