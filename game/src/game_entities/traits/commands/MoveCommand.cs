namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class MoveCommand : Command {
  public void Move(MoveTrait moveTrait, GameCell targetCell) {
    moveTrait.Move(targetCell);
  }

  public override bool IsAvailable() => true;
}
