namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public partial class MoveCommand : Command {
  [Export] public MoveTrait EntityMoveTrait { get; private set; }
  public override void _Ready() {
    base._Ready();

    EntityMoveTrait.MoveFinished += Finish;
  }
  public bool TryMove(GameCell targetCell) {
    if (EntityMoveTrait.TryMove(targetCell)) {
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
