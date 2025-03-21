namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class GameplayEffectModifier : Resource {
  [Export] public GameplayAttribute Attribute { get; private set; }
  [Export] public AttributeModifierType ModifierType { get; private set; }
  [Export] public ModifierMagnitudeResource ModifierMagnitude { get; private set; }
}