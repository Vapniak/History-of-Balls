namespace HOB.GameEntity;

using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class MoveTrait : Trait {
  [Signal] public delegate void MoveFinishedEventHandler();
  private GameCell[] _reachableCells;
  private Vector3 _targetPosition;
  private bool _move;
  private GameCell[] _path;
  private int _pathIndex;
  public override void _PhysicsProcess(double delta) {
    base._PhysicsProcess(delta);

    if (_move) {
      _targetPosition = Entity.GameBoard.GetCellRealPosition(_path[_pathIndex]);
      if (Entity.GetPosition().DistanceTo(_targetPosition) < .1) {
        if (_pathIndex < _path.Length - 1) {
          _pathIndex++;
        }
        else {
          _move = false;
          _reachableCells = null;
          EmitSignal(SignalName.MoveFinished);
        }
      }

      if (_targetPosition.DistanceSquaredTo(Entity.GetPosition()) > 1) {
        Entity.LookAt(_targetPosition);
      }

      Entity.SetPosition(Entity.GetPosition().Lerp(_targetPosition, (float)delta * GetStat<MovementStats>().MoveSpeed));
    }
  }

  // TODO: function to filter reachble cells
  public GameCell[] GetReachableCells() {
    var cells = Entity.GameBoard.FindReachableCells(Entity.Cell, GetStat<MovementStats>().MovePoints, IsReachable);
    _reachableCells = cells;
    return cells;
  }
  public bool TryMove(GameCell targetCell) {
    if (!_reachableCells.Contains(targetCell)) {
      return false;
    }

    // FIXME: path bottle necks when target cell is not reachable, and try to embed path into find reachable cells to not find path twice
    var path = Entity.GameBoard.FindPath(Entity.Cell, targetCell, GetStat<MovementStats>().MovePoints, IsReachable);
    if (path == null) {
      return false;
    }

    _path = path;
    _pathIndex = 0;
    Entity.Cell = path.Last();
    _move = true;


    return true;
  }

  public bool IsReachable(GameCell start, GameCell end) {
    return
    Entity.GameBoard.GetEntitiesOnCell(end).Length == 0 &&
    Entity.GameBoard.GetSetting(end).MoveCost > 0 &&
    Entity.GameBoard.GetEdgeType(start, end) != GameCell.EdgeType.Cliff &&
    Entity.GameBoard.GetSetting(end).Elevation >= 0;
  }
}
