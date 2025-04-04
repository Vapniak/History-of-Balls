namespace GameplayAbilitySystem;

using Godot;

public partial class GameplayModifierEvaluatedData : RefCounted {
  public GameplayAttribute Attribute { get; private set; }
  public float Magnitude { get; private set; }
  public AttributeModifierType ModifierType { get; private set; }

  public GameplayModifierEvaluatedData(GameplayAttribute attribute, float magnitude, AttributeModifierType modifierType) {
    Attribute = attribute;
    Magnitude = magnitude;
    ModifierType = modifierType;
  }
}