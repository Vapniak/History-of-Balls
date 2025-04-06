namespace GameplayAbilitySystem;

using Godot;

public partial class AggregatorMod : RefCounted {
  public float Magnitude { get; private set; }

  public AggregatorMod(float magnitude) {
    Magnitude = magnitude;
  }
}