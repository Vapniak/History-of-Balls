namespace GameplayAbilitySystem;

using System;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class GameplayEffectDefinition : Resource {
  [Export] public DurationPolicy DurationPolicy { get; private set; }
  [Export] public Array<GameplayAttributeModifier>? AttributeModifiers { get; private set; }

  [Export] private ModifierMagnitudeResource? DurationModifier { get; set; }
  [Export] private float DurationMultiplier { get; set; }
  [Export] private DurationStrategy? DurationStrategy { get; set; }


  public DurationStrategy? CreateDurationStrategy() {
    return DurationStrategy?.Duplicate() as DurationStrategy;
  }
  public float GetDuration(GameplayEffectInstance gameplayEffectInstance) {
    if (DurationModifier == null) {
      return DurationMultiplier;
    }
    else {
      return DurationModifier.CalculateMagnitude(gameplayEffectInstance).GetValueOrDefault(1) * DurationMultiplier;
    }
  }
}
