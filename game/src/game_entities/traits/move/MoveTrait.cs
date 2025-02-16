namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public partial class MoveTrait : Trait {
  [Signal] public delegate void MoveFinishedEventHandler();

  public GameCell[] GetReachableCells(MovementType movementType) {
    return Entity.GameBoard.FindReachableCells(Entity.Cell, GetStat<MovementStats>().MovePoints, movementType.IsCellReachable);
  }

  public GameCell[] FindPath(GameCell cell, MovementType movementType) {
    return Entity.GameBoard.FindPath(Entity.Cell, cell, GetStat<MovementStats>().MovePoints, movementType.IsCellReachable);
  }

  public bool TryMove(GameCell targetCell, MovementType movementType) {
    var path = FindPath(targetCell, movementType);

    void onMoveFinished() {
      EmitSignal(SignalName.MoveFinished);
      movementType.MoveFinished -= onMoveFinished;
    }

    movementType.MoveFinished += onMoveFinished;

    movementType.StartMoveOn(path);

    return true;
  }
}
