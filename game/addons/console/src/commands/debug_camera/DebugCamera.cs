namespace Console;

using Godot;
using System;

public partial class DebugCamera : Camera3D {
  [Export] public float MoveSpeed { get; set; } = 5.0f;
  [Export] public float SprintMultiplier { get; set; } = 3.0f;
  [Export] public float LookSensitivity { get; set; } = 0.2f;
  [Export] public bool InvertY { get; set; } = false;

  private Vector2 _mouseRelative;

  private Camera3D? _currentCamera;
  private bool _active;

  public override void _Input(InputEvent @event) {
    if (!_active) {
      return;
    }

    if (@event is InputEventMouseMotion mouseEvent) {
      _mouseRelative = mouseEvent.Relative;
    }
  }

  public override void _Process(double delta) {
    if (!_active) {
      return;
    }

    var yaw = _mouseRelative.X * LookSensitivity * (float)delta;
    var pitch = _mouseRelative.Y * LookSensitivity * (float)delta * (InvertY ? 1 : -1);

    RotateY(Mathf.DegToRad(-yaw));
    RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-pitch));

    _mouseRelative = Vector2.Zero;

    var direction = Vector3.Zero;
    var speed = MoveSpeed;

    if (Input.IsKeyPressed(Key.Shift))
      speed *= SprintMultiplier;

    if (Input.IsKeyPressed(Key.W))
      direction -= Transform.Basis.Z;
    if (Input.IsKeyPressed(Key.S))
      direction += Transform.Basis.Z;
    if (Input.IsKeyPressed(Key.A))
      direction -= Transform.Basis.X;
    if (Input.IsKeyPressed(Key.D))
      direction += Transform.Basis.X;
    if (Input.IsKeyPressed(Key.E))
      direction += Vector3.Up;
    if (Input.IsKeyPressed(Key.Q))
      direction += Vector3.Down;

    if (direction != Vector3.Zero) {
      direction = direction.Normalized();
      Position += direction * speed * (float)delta;
    }
  }


  [ConsoleCommand("devcam", Description = "Toggles between the current camera and a free-flying camera")]
  public void ToggleDebugCamera() {
    _active = !_active;

    if (_active) {
      _currentCamera = GetViewport().GetCamera3D();

      Fov = _currentCamera.Fov;
      Position = _currentCamera.GlobalPosition;
      Rotation = _currentCamera.GlobalRotation;
      Visible = true;
      MakeCurrent();
      Input.MouseMode = Input.MouseModeEnum.Captured;

      Console.Instance.Print("Dev camera on");
    }
    else {
      _currentCamera?.MakeCurrent();
      Visible = false;

      Input.MouseMode = Input.MouseModeEnum.Visible;

      Console.Instance.Print("Dev camera off");
    }
  }
}
