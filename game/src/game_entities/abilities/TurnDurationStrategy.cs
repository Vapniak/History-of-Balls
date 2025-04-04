namespace HOB;

using System.Linq;
using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class TurnDurationStrategy : DurationStrategy {
  [Export] public Array<Tag> TickAt { get; private set; } = new();
  [Export] public bool TickAtOwnTurn { get; private set; }

  public int TotalTicks { get; private set; }
  private int _ticksLeft;
  public override bool IsExpired => _ticksLeft <= 0;

  public override void Initialize(float magnitude) {
    TotalTicks = _ticksLeft = (int)magnitude;
  }
  public override void Left(float value) => _ticksLeft = (int)value;
  public override void Reset() => _ticksLeft = TotalTicks;
  public override void Tick(ITickContext tickContext) {
    if (tickContext is TurnTickContext turnTick) {
      if (turnTick.OwnTurn == TickAtOwnTurn) {
        if (TickAt.Any(turnTick.Event.IsExact)) {
          _ticksLeft--;
        }
      }
    }
  }
}
