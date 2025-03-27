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

  public override void PostGameplayEffectExecute(GameplayAttribute attribute, ref float value) {
    // TODO: pre attribute change handlers to make generic funcionality like clamp value
  }
}
