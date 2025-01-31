namespace HOB.GameEntity;

using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class MoveCommand : Command {
  public GameCell[] CellsToMove { get; set; }
  public bool TryMove(MoveTrait moveTrait, GameCell targetCell) {
    if (CellsToMove.Contains(targetCell)) {
      moveTrait.Move(targetCell);
      return true;
    }
    return false;
  }

  public override bool IsAvailable() => true;
}
