namespace HOB;

using GameplayAbilitySystem;
using Godot;

[GlobalClass]
public partial class TurnDurationStrategy : DurationStrategy {
  [Export] public TurnPhase TickAt { get; private set; }
  [Export] public bool TickAtOwnTurn { get; private set; }

  public int TotalTicks { get; private set; }
  private int _ticksLeft;
  public override bool IsExpired => _ticksLeft <= 0;

  public override void Initialize(float magnitude) => TotalTicks = _ticksLeft = (int)magnitude;
  public override void Left(float value) => _ticksLeft = (int)value;
  public override void Reset() => _ticksLeft = TotalTicks;
  public override void Tick(TickContext tickContext) {
    if (tickContext is TurnTickContext turnTick) {
      if (turnTick.OwnTurn == TickAtOwnTurn) {
        if (TickAt.HasFlag(turnTick.TurnPhase)) {
          _ticksLeft--;
        }
      }
    }
  }
}