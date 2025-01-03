namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class MoveTrait : Trait, IGetTraitData<MoveTraitData> {
  [Signal] public delegate void MoveFinishedEventHandler();

  [Export] public MoveTraitData Data { get; private set; }
  [Export] private float _moveSpeed = 10;
  private Vector3 _targetPosition;
  private bool _move;

  public override void _PhysicsProcess(double delta) {
    base._PhysicsProcess(delta);

    if (_move) {
      GetEntity().GlobalPosition += _targetPosition.Normalized() * (float)delta * _moveSpeed;
      if (GetEntity().GlobalPosition == _targetPosition) {
        EmitSignal(SignalName.MoveFinished);
        _move = false;
      }
    }
  }
  public void Move(Vector3 targetPosition) {
    _targetPosition = targetPosition;
    _move = true;
  }
}
