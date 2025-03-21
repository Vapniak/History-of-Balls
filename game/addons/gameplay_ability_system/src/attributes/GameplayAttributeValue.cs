namespace GameplayAbilitySystem;

using Godot;
public partial class GameplayAttributeValue : Resource {
  [Export] public float BaseValue { get; set; }
  public float CurrentValue => BaseValue;
}
