namespace GameplayAbilitySystem;

using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GameplayEffectDefinition : Resource {
  [Export] public DurationPolicy DurationPolicy { get; private set; }
  [Export] public ModifierMagnitudeResource? DurationModifier { get; private set; }
  [Export] public float DurationMultiplier { get; private set; }
  [Export] public DurationStrategy? DurationStrategy { get; private set; }
  [Export] public Array<GameplayEffectModifier>? Modifiers { get; private set; }
}