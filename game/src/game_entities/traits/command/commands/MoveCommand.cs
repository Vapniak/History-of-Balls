namespace HOB.GameEntity;

using System.Linq;
using Godot;

[GlobalClass]
public partial class MoveCommand : Command {
  [Export] public MoveTrait EntityMoveTrait { get; private set; }
  [Export] public MovementType MovementType { get; private set; }
  public override void _Ready() {
    base._Ready();

    EntityMoveTrait.MoveFinished += Finish;

    MovementType = MovementType.Duplicate() as MovementType;
    MovementType.MoveTrait = EntityMoveTrait;
  }
  public bool TryMove(GameCell targetCell) {
    if (IsAvailable()) {
      Use();
      EntityMoveTrait.Move(targetCell, MovementType);
      return true;
    }

    return false;
  }

  public GameCell[] GetReachableCells() {
    return EntityMoveTrait.GetReachableCells(MovementType);
  }

  public GameCell[] FindPathTo(GameCell cell) {
    return EntityMoveTrait.FindPath(cell, MovementType);
  }

  public override bool IsAvailable() {
    var attacked = false;
    if (CommandTrait.TryGetCommand<AttackCommand>(out var attack)) {
      attacked = attack.UsedThisRound;
    }

    return base.IsAvailable() && !attacked;
  }
}
