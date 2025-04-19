namespace HOB;

using GameplayAbilitySystem;
using Godot;

[GlobalClass]
public partial class IncomeAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute IncomeAmount { get; private set; }
  public override GameplayAttribute[] GetAttributes() => new[] { IncomeAmount };
}