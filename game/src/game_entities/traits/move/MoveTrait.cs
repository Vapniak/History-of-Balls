namespace HOB.GameEntity;

using Godot;
using HexGridMap;
using System;

[GlobalClass]
public partial class MoveTrait : Trait {
  [Signal] public delegate void MoveFinishedEventHandler();

  [Export] public MoveTraitData Data { get; private set; }
  [Export] private float _moveSpeed = 10;
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
  public void Move(GameCell targetCell) {
    var pos = targetCell.Position;
    _targetPosition = new(pos.X, 0, pos.Y);
    _move = true;

    Entity.LookAt(_targetPosition, Vector3.Up);

    Entity.Cell = targetCell;
  }
}
