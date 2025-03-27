namespace HOB;

using GameplayAbilitySystem;
using Godot;
using System;

[GlobalClass]
public partial class PlayerAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute PrimaryResource { get; private set; } = new();
  [Export] public GameplayAttribute SecondaryResource { get; private set; } = new();

  public override GameplayAttribute[] GetAttributes() => new[] { PrimaryResource, SecondaryResource };
}
