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
  [Export] public int MovePoints { get; private set; } = 0;
  [Export] private float _moveSpeed = 10;

  public GameCell[] CellsToMove;
  private Vector3 _targetPosition;
  private bool _move;
  private GameCell[] _path;
  private int _pathIndex;
  public override void _PhysicsProcess(double delta) {
    base._PhysicsProcess(delta);

    if (_move) {
      _targetPosition = _path[_pathIndex].GetRealPosition();
      if (Entity.GetPosition().DistanceTo(_targetPosition) < 1) {
        if (_pathIndex < _path.Length - 1) {
          _pathIndex++;
        }
      }

      if (_targetPosition.DistanceSquaredTo(Entity.GetPosition()) > 1) {
        Entity.LookAt(_targetPosition);
      }
      Entity.SetPosition(Entity.GetPosition().Lerp(_targetPosition, (float)delta * _moveSpeed));
    }
  }
  public bool TryMove(GameCell[] path) {
    _path = path;
    _pathIndex = 0;
    Entity.Cell = path.Last();
    _move = true;
    return true;
  }
}
