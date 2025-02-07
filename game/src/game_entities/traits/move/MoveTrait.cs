namespace HOB.GameEntity;

using Godot;
using HexGridMap;
using System;
using System.IO;
using System.Linq;

[GlobalClass]
public partial class MoveTrait : Trait {
  [Signal] public delegate void MoveFinishedEventHandler();


  // TODO: move that data somewhere else
  // TODO: make some simple entity editor plugin
  [Export] public uint MovePoints { get; private set; } = 0;
  [Export] private float _moveSpeed = 10;

  private GameCell[] _reachableCells;
  private Vector3 _targetPosition;
  private bool _move;
  private GameCell[] _path;
  private int _pathIndex;
  public override void _PhysicsProcess(double delta) {
    base._PhysicsProcess(delta);

    if (_move) {
      _targetPosition = _path[_pathIndex].GetRealPosition();
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

      Entity.SetPosition(Entity.GetPosition().Lerp(_targetPosition, (float)delta * _moveSpeed));
    }
  }

  // TODO: function to filter reachble cells
  public GameCell[] GetReachableCells(GameBoard board) {
    var cells = board.FindReachableCells(Entity.Cell, MovePoints, (start, end) => IsReachable(start, end, board));
    _reachableCells = cells;
    return cells;
  }
  public bool TryMove(GameCell targetCell, GameBoard board) {
    var path = board.FindPath(Entity.Cell, targetCell, MovePoints, (start, end) => IsReachable(start, end, board));
    if (path == null || !_reachableCells.Contains(path.Last())) {
      return false;
    }

    _path = path;
    _pathIndex = 0;
    Entity.Cell = path.Last();
    _move = true;
    return true;
  }

  public bool IsReachable(GameCell start, GameCell end, GameBoard board) {
    return board.GetEntitiesOnCell(end).Length == 0 && end.MoveCost > 0;
  }
}
