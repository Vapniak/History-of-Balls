namespace HOB.GameEntity;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public abstract partial class MoveTrait : Trait {
  [Signal] public delegate void MoveFinishedEventHandler();

  public virtual GameCell[] GetReachableCells() {
    return Entity.Cell.ExpandSearch(GetStat<MovementStats>().MovePoints, IsCellReachable);
  }

  public virtual GameCell[] FindPath(GameCell cell) {
    return Entity.Cell.FindPathTo(cell, GetStat<MovementStats>().MovePoints, IsCellReachable);
  }

  public virtual async Task Move(GameCell targetCell) {
    EmitSignal(SignalName.MoveFinished);
  }

  public abstract bool IsCellReachable(GameCell from, GameCell to);
}
