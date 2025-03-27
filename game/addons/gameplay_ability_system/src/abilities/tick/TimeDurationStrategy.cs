namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class TimeDurationStrategy : DurationStrategy {
  private float _remaining;
  public float Total { get; private set; }

  public override bool IsExpired => _remaining <= 0;
  public override void Initialize(float magnitude) {
    Total = _remaining = magnitude;
  }
  public override void Left(float value) {
    _remaining = value;
  }
  public override void Reset() => _remaining = Total;
  public override void Tick(ITickContext tickContext) {
    if (tickContext is TimeTickContext timeTick) {
      _remaining -= timeTick.DeltaTime;
    }
  }
}
