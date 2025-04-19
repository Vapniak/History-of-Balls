namespace HOB;

using GameplayAbilitySystem;
using Godot;

[GlobalClass]
public partial class ProcessResourcesAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute ProcessTime { get; private set; }
  [Export] public GameplayAttribute ProcessedValue { get; private set; }
  [Export] public GameplayAttribute ProducedValue { get; private set; }

  public override GameplayAttribute[] GetAttributes() => new[] { ProcessedValue, ProducedValue, ProcessTime };
}