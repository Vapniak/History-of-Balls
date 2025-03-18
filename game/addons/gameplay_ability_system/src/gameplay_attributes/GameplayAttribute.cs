namespace GameplayAbilitySystem;

using Godot;
using System;

[GlobalClass]
public partial class GameplayAttribute : Resource {
  [Export] public float MinValue { get; private set; }
  [Export] public float MaxValue { get; private set; }
  [Export] public float BaseValue { get; private set; }
  public float BuffedValue { get; private set; }

  public float CurrentValue => BaseValue + BuffedValue;
}
