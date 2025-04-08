namespace GameplayAbilitySystem;

using System;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class GameplayAttributeModifier : Resource {
  [Export] public GameplayAttribute Attribute { get; private set; } = new();
  [Export] public Array<GameplayEffectModifier> Modifiers { get; private set; } = new();
}