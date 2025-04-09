namespace GameplayAbilitySystem;

using Godot;

[GlobalClass, Tool]
public partial class GameplayEffectModifier : Resource {
  [Export] public AttributeModifierType ModifierType { get; private set; }
  [Export] public ModifierMagnitudeResource? ModifierMagnitude { get; private set; }

  public float GetMagnitude(GameplayEffectInstance geInstance) {
    return ModifierMagnitude == null ? 0 : ModifierMagnitude.CalculateMagnitude(geInstance).GetValueOrDefault(0);
  }
}
