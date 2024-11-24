namespace HOB;

using Godot;
using System;

public partial class Player : CharacterBody3D {
  [Export] private float _minHeight = 10;
  [Export] private float _baseY = 20;
  [Export] private float _maxHeight = 100;
  [Export] private float _zoomSpeed = 10;
  [Export] private float _cameraLerpSpeed = 10;

  [Export] private float _minAngle = -45;
  [Export] private float _maxAngle = -90;

  [Export] private float _moveSpeed = 10;
  private float _targetAngle;
  private float _currentAngle;

  private float _currentHeight;

  private bool _dragging;

  private Vector2 _mouseMovement;

  public override void _Ready() {
    _targetAngle = _minAngle;
    _currentHeight = _minHeight;
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left) {
      if (!_dragging && mouseEvent.Pressed) {
        _dragging = true;
      }
      else if (_dragging && !mouseEvent.Pressed) {
        _dragging = false;
      }
    }
    else {
      if (@event is InputEventMouseMotion motionEvent && _dragging) {
        _mouseMovement = motionEvent.Relative;
      }
      else {
        _mouseMovement = Vector2.Zero;
      }
    }
  }

  public override void _Process(double delta) {
    if (Input.IsActionJustPressed("zoom_in")) {
      _currentHeight -= (float)delta * _zoomSpeed;
    }
    else if (Input.IsActionJustPressed("zoom_out")) {
      _currentHeight += (float)delta * _zoomSpeed;
    }

    _currentHeight = Mathf.Clamp(_currentHeight, _minHeight, _maxHeight);

    _targetAngle = _minAngle + ((_maxAngle - _minAngle) * _currentHeight / _maxHeight);
    _currentAngle = Mathf.Lerp(_currentAngle, _targetAngle, (float)delta * _cameraLerpSpeed);

    GlobalPosition = GlobalPosition.Lerp(new(GlobalPosition.X, _baseY + _currentHeight, GlobalPosition.Z), (float)delta * _cameraLerpSpeed);

    Basis = Basis.Identity;
    RotateX(Mathf.DegToRad(_currentAngle));

    Velocity = new Vector3(-_mouseMovement.X, 0, -_mouseMovement.Y) * _moveSpeed;
    MoveAndSlide();
  }
}
