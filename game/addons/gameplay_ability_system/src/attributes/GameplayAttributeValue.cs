namespace GameplayAbilitySystem;

using Godot;

public sealed partial class GameplayAttributeValue : Resource {
  public float BaseValue { get; set; }
  public float CurrentValue { get; set; }
}
