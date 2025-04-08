namespace GameplayAbilitySystem;


using Godot;

[GlobalClass, Tool]
public partial class SimpleFloatModifierMagnitude : ModifierMagnitudeResource {

  [Export] public float Magnitude { get; private set; }
  public override float? CalculateMagnitude(GameplayEffectInstance effectInstance) {
    return Magnitude;
  }
  public override void Initialize(GameplayEffectInstance effectInstance) {

  }
}