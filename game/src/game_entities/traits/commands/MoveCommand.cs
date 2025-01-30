namespace HOB.GameEntity;

using GameplayFramework;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class MoveCommand : Command {
  public GameCell[] CellsToMove { get; set; }
  public bool TryMove(GameCell targetCell) {
    if (CellsToMove.Contains(targetCell)) {
      Entity.GetTrait<MoveTrait>().Move(targetCell);
      return true;
    }
    return false;
  }

  public override bool IsAvailable() => true;
}
