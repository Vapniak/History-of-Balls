namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class GameplayEffectPeriod : Resource {
  [Export] public float Period { get; set; }
  [Export] public bool ExecuteOnApplication { get; set; }
  [Export] public DurationStrategy? PeriodStrategy { get; private set; }
}