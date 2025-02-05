namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public partial class MoveCommand : Command {
  // TODO: wait for finish until you use next command
  public bool Moved { get; private set; }

  public MoveTrait EntityMoveTrait { get; private set; }
  public override void _Ready() {
    base._Ready();

    EntityMoveTrait = GetEntity().GetTrait<MoveTrait>();
    EntityMoveTrait.MoveFinished += Finish;
  }
  public bool TryMove(GameCell[] path) {
    if (EntityMoveTrait.TryMove(path)) {
      Start();
      Moved = true;
      return true;
    }

    Moved = false;
    return false;
  }

  public override void OnRoundChanged(int roundNumber) {
    base.OnRoundChanged(roundNumber);

    Moved = false;
  }

  public override bool IsAvailable() {
    var attacked = false;
    if (CommandTrait.TryGetCommand<AttackCommand>(out var attack)) {
      attacked = attack.Attacked;
    }

    return base.IsAvailable() && !Moved && !attacked;
  }
}
