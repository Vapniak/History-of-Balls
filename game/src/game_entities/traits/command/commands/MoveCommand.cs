namespace HOB.GameEntity;

using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class MoveCommand : Command {
  // TODO: wait for finish until you use next command
  public bool Moved { get; private set; }
  public bool TryMove(GameCell[] path) {
    if (GetEntity().GetTrait<MoveTrait>().TryMove(path)) {
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

    return !Moved && !attacked;
  }
}
