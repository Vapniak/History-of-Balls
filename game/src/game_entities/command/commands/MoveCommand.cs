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

  public bool TryMove(IMatchController caller, GameCell targetCell) {
    var path = FindPathTo(targetCell);
    if (CanBeUsed(caller) && path.Length > 0) {
      Use();
      MoveTrait.Move(path);
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

  public override bool CanBeUsed(IMatchController caller) {
    var attacked = false;
    if (CommandTrait.TryGetCommand<AttackCommand>(out var attack)) {
      attacked = attack.UsedThisRound;
    }

    return base.CanBeUsed(caller) && !attacked;
  }
}
