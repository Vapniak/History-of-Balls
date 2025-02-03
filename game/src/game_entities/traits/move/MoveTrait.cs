namespace HOB.GameEntity;

using Godot;
using HexGridMap;
using System;
using System.Linq;

[GlobalClass]
public partial class MoveTrait : Trait {
  [Signal] public delegate void MoveFinishedEventHandler();


  // TODO: move that data somewhere else
  // TODO: make some simple entity editor plugin
  [Export] public uint MovePoints { get; private set; } = 0;
  [Export] private float _moveSpeed = 10;

  public GameCell[] CellsToMove;
  private Vector3 _targetPosition;
  private bool _move;

  public override void _PhysicsProcess(double delta) {
    base._PhysicsProcess(delta);

    if (_move) {
      Entity.GlobalPosition = Entity.GlobalPosition.Lerp(_targetPosition, (float)delta * _moveSpeed);
      if (Entity.GlobalPosition == _targetPosition) {
        EmitSignal(SignalName.MoveFinished);
        _move = false;
      }
    }
  }
  public bool TryMove(GameCell targetCell) {
    if (!CellsToMove.Contains(targetCell)) {
      return false;
    }

    var pos = targetCell.Position;
    _targetPosition = new(pos.X, 0, pos.Y);
    _move = true;

    Entity.LookAt(_targetPosition, Vector3.Up);

    Entity.Cell = targetCell;

    return true;
  }
}
