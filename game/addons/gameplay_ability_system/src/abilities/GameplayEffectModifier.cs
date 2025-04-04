namespace GameplayAbilitySystem;

using Godot;

[GlobalClass, Tool]
public partial class GameplayEffectModifier : Resource {
  [Export] public GameplayAttribute? Attribute { get; private set; }
  [Export] public AttributeModifierType ModifierType { get; private set; }
  [Export] public ModifierMagnitudeResource? ModifierMagnitude { get; private set; }
  [Export] private float Coefficient { get; set; } = 1;

  public float GetMagnitude(GameplayEffectInstance geInstance) {
    return (ModifierMagnitude == null ? 1 : ModifierMagnitude.CalculateMagnitude(geInstance)) * Coefficient;
  }
}
