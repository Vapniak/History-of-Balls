namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public abstract partial class ModifierMagnitudeResource : Resource {
  public abstract void Initialize(GameplayEffectInstance effectInstance);
  public abstract float? CalculateMagnitude(GameplayEffectInstance effectInstance);
}