namespace Console;

using Godot;

public partial class DebugCamera : Camera3D {
  [Export] public float MoveSpeed { get; set; } = 5.0f;
  [Export] public float SprintMultiplier { get; set; } = 3.0f;
  [Export] public float LookSensitivity { get; set; } = 0.2f;
  [Export] public float SmoothingFactor { get; set; } = 10.0f; // New smoothing parameter
  [Export] public bool InvertY { get; set; } = false;

  private Vector2 _mouseRelative;
  private Vector2 _targetRotation; // Stores accumulated rotation for smoothing
  private Vector2 _currentRotation; // Current smoothed rotation

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

    float deltaFloat = (float)delta;

    // Accumulate mouse input for smooth rotation
    _targetRotation.X += _mouseRelative.X * LookSensitivity;
    _targetRotation.Y += _mouseRelative.Y * LookSensitivity * (InvertY ? 1 : -1);
    _mouseRelative = Vector2.Zero;

    // Apply smoothing using linear interpolation
    _currentRotation.X = Mathf.Lerp(_currentRotation.X, _targetRotation.X, SmoothingFactor * deltaFloat);
    _currentRotation.Y = Mathf.Lerp(_currentRotation.Y, _targetRotation.Y, SmoothingFactor * deltaFloat);

    // Reset rotation to avoid gimbal lock
    Transform = Transform.Orthonormalized();

    // Apply smoothed rotation
    RotateY(Mathf.DegToRad(-_currentRotation.X * deltaFloat));
    RotateObjectLocal(new Vector3(1, 0, 0), Mathf.DegToRad(-_currentRotation.Y * deltaFloat));

    // Reset the accumulated rotation after applying it
    _targetRotation -= _currentRotation;
    _currentRotation = Vector2.Zero;

    // Movement code remains the same
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
      Position += direction * speed * deltaFloat;
    }
  }

  [ConsoleCommand("devmode", Description = "Toggles between the current camera and a free-flying camera")]
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

      // Reset rotation accumulators when activating
      _targetRotation = Vector2.Zero;
      _currentRotation = Vector2.Zero;

      ProjectSettings.SetSetting("application/devmode", true);
      Console.Instance.Print("Dev camera on");
    }
    else {
      _currentCamera?.MakeCurrent();
      Visible = false;

      Input.MouseMode = Input.MouseModeEnum.Visible;
      ProjectSettings.SetSetting("application/devmode", false);

      Console.Instance.Print("Dev camera off");
    }
  }
}
