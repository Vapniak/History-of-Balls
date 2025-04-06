namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class GameplayEffectPeriod : Resource {
  [Export] public float Period { get; set; }
  [Export] public bool ExecuteOnApplication { get; set; }
  [Export] private DurationStrategy? PeriodStrategy { get; set; }

  public DurationStrategy? CreatePeriodStrategy() {
    return PeriodStrategy?.Duplicate() as DurationStrategy;
  }
}