namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public partial class MoveCommand : Command {
  [Export] public MoveTrait EntityMoveTrait { get; private set; }
  public override void _Ready() {
    base._Ready();

    EntityMoveTrait.MoveFinished += Finish;
  }
  public bool TryMove(GameCell targetCell, GameBoard board) {
    if (EntityMoveTrait.TryMove(targetCell, board)) {
      Use();
      return true;
    }

    return false;
  }

  public override bool IsAvailable() {
    var attacked = false;
    if (CommandTrait.TryGetCommand<AttackCommand>(out var attack)) {
      attacked = attack.UsedThisRound;
    }

    return base.IsAvailable() && !attacked;
  }
}
