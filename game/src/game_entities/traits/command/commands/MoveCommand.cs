namespace HOB.GameEntity;

using System.Linq;
using Godot;

[GlobalClass]
public partial class MoveCommand : Command {
  [Export] public MoveTrait MoveTrait { get; private set; }
  public override void _Ready() {
    base._Ready();

    MoveTrait.MoveFinished += Finish;
  }
  public bool TryMove(GameCell targetCell) {
    if (IsAvailable() && FindPathTo(targetCell).Length > 0) {
      Use();
      MoveTrait.Move(targetCell);
      return true;
    }

    return false;
  }

  public GameCell[] GetReachableCells() {
    return MoveTrait.GetReachableCells();
  }

  public GameCell[] FindPathTo(GameCell cell) {
    return MoveTrait.FindPath(cell);
  }

  public override bool IsAvailable() {
    var attacked = false;
    if (CommandTrait.TryGetCommand<AttackCommand>(out var attack)) {
      attacked = attack.UsedThisRound;
    }

    return base.IsAvailable() && !attacked;
  }
}
