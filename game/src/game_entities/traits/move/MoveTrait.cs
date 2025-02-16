namespace HOB.GameEntity;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class MoveTrait : Trait {
  [Signal] public delegate void MoveFinishedEventHandler();

  public GameCell[] GetReachableCells(MovementType movementType) {
    return Entity.Cell.ExpandSearch(GetStat<MovementStats>().MovePoints, movementType.IsCellReachable);
  }

  public GameCell[] FindPath(GameCell cell, MovementType movementType) {
    return Entity.Cell.FindPathTo(cell, GetStat<MovementStats>().MovePoints, movementType.IsCellReachable);
  }

  public async Task Move(GameCell targetCell, MovementType movementType) {
    var path = FindPath(targetCell, movementType);

    void onMoveFinished() {
      EmitSignal(SignalName.MoveFinished);
      movementType.MoveFinished -= onMoveFinished;
    }

    movementType.MoveFinished += onMoveFinished;

    await movementType.StartMoveOn(path);
  }
}
