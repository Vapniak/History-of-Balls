namespace GameplayAbilitySystem;


using Godot;

[GlobalClass, Tool]
public partial class SimpleFloatModifierMagnitude : ModifierMagnitudeResource {
  [Export] private Curve? ScalingFunction { get; set; }
  public override float? CalculateMagnitude(GameplayEffectInstance effectInstance) {
    return ScalingFunction?.Sample(effectInstance.Level);
  }
  public override void Initialize(GameplayEffectInstance effectInstance) {

  }
}