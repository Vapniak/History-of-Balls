namespace HOB;

using GameplayAbilitySystem;
using Godot;
using HOB;

[GlobalClass]
public partial class HealthAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute HealthAttribute { get; private set; }

  public override GameplayAttribute[] GetAttributes() {
    return new[] { HealthAttribute };
  }
}