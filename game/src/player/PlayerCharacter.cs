namespace HOB;

using GameplayFramework;
using Godot;

public partial class PlayerCharacter : Node3D, IPlayerControllable {
  [Export] public Camera3D Camera { get; private set; }
  [Export] public int EdgeMarginPixels { get; private set; } = 50;
  [Export] private float _acceleration = 10f;
  [Export] private float _moveSpeedMinZoom = 400f, _moveSpeedMaxZoom = 100f;
  [Export] private float _friction = 10f;
  [Export] private float _stopSpeed = 5f;

  [ExportGroup("Zoom")]
  [Export] public bool AllowZoom { get; private set; } = true;
  [Export] private float _zoomLerpSpeed = 5f;
  [Export(PropertyHint.Range, "0,1")] private float _zoomSpeed = 0.1f;
  [Export] private Vector3 _minZoomOffset = new(0, 300, 100);
  [Export] private Vector3 _maxZoomOffset = new(0, 40, 10);
  [Export] private Curve _distanceCurve;

  [ExportGroup("Panning")]
  [Export] public bool AllowPan { get; private set; } = true;
  [Export] private float _panSpeedMulti = 20f;

  public float SpeedMulti { get; private set; } = 1;
  public Vector3 Velocity { get; private set; }
  public float Zoom { get; private set; } = 1f;

  public float MoveSpeed => Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, Zoom) * SpeedMulti;

  private float _targetZoom = 1f;
  private float _zoomVelocity;
  private Vector3 _cameraVelocity;
  private Vector3 _panVelocity;

  private bool _isMovingToPosition;
  private Tween _moveTween;

  public override void _Process(double delta) {
    UpdateCamera((float)delta);
  }

  public void AdjustZoom(float zoomDelta) {
    zoomDelta = Mathf.Clamp(zoomDelta, -1, 1);
    _targetZoom = Mathf.Clamp(_targetZoom + (zoomDelta * _zoomSpeed), 0, 1);
  }

  public void HandleDirectionalMovement(double delta, Vector2 horizontalDirection) {
    if (_isMovingToPosition) {
      return;
    }

    var direction = Transform.Basis * new Vector3(horizontalDirection.X, 0f, horizontalDirection.Y).Normalized();
    Velocity = Velocity.Lerp(direction * MoveSpeed, (float)delta * _acceleration);
  }

  public void HandlePanning(double delta, Vector2 mouseDisplacement) {
    if (_isMovingToPosition) {
      return;
    }

    var targetVelocity = new Vector3(-mouseDisplacement.X, 0f, -mouseDisplacement.Y) * MoveSpeed * _panSpeedMulti / (float)delta;
    Velocity = new Vector3(
        SmoothDamp(Velocity.X, targetVelocity.X, ref _panVelocity.X, 0.1f, float.MaxValue, (float)delta),
        Velocity.Y,
        SmoothDamp(Velocity.Z, targetVelocity.Z, ref _panVelocity.Z, 0.1f, float.MaxValue, (float)delta)
    );
  }

  public void MoveToPosition(Vector3 position, double duration, Tween.TransitionType transitionType) {
    _moveTween = CreateTween();
    _moveTween.TweenProperty(this, "global_position", position, duration).SetTrans(transitionType);
    _isMovingToPosition = true;
    _moveTween.Finished += () => _isMovingToPosition = false;
  }

  public void CancelMoveToPosition() {
    _isMovingToPosition = false;
    if (IsInstanceValid(_moveTween)) {
      _moveTween.Kill();
    }
  }

  public void Friction(double delta) {
    var vel = Velocity;
    vel.Y = 0;

    var control = vel.Length() < _stopSpeed ? _stopSpeed : vel.Length();
    var drop = control * _friction * (float)delta;

    if (drop <= 0) {
      return;
    }

    var newSpeed = vel.Length() - drop;

    if (newSpeed < 0) {
      newSpeed = 0;
    }

    if (vel.LengthSquared() > 0) {
      newSpeed /= vel.Length();
    }

    Velocity *= new Vector3(newSpeed, 1, newSpeed);
  }

  public void CenterPositionOn(Aabb aabb) {
    GlobalPosition = new(aabb.GetCenter().X, GlobalPosition.Y, aabb.GetCenter().Z);
  }

  public void ClampPosition(Aabb aabb) {
    var pos = GlobalPosition;
    pos = pos.Clamp(aabb.GetCenter() - (aabb.Size / 2), aabb.GetCenter() + (aabb.Size / 2));
    GlobalPosition = new(pos.X, GlobalPosition.Y, pos.Z);
  }

  public void Move(double delta) {
    GlobalTranslate(Velocity * (float)delta);
    SpeedMulti = 1f;
  }

  public void ApplySpeedMulti() {
    SpeedMulti = 2f;
  }

  public T GetCharacter<T>() where T : Node => this as T;

  private void UpdateCamera(float delta) {
    Zoom = SmoothDamp(Zoom, _targetZoom, ref _zoomVelocity, 0.1f, float.MaxValue, delta);

    var targetDistance = Mathf.Lerp(_minZoomOffset.Z, _maxZoomOffset.Z, _distanceCurve.Sample(Zoom));
    var targetHeight = Mathf.Lerp(_minZoomOffset.Y, _maxZoomOffset.Y, Zoom);
    var targetPosition = new Vector3(0, targetHeight, Mathf.Max(targetDistance, 1));

    Camera.Position = new Vector3(
        SmoothDamp(Camera.Position.X, targetPosition.X, ref _cameraVelocity.X, 0.1f, float.MaxValue, delta),
        SmoothDamp(Camera.Position.Y, targetPosition.Y, ref _cameraVelocity.Y, 0.1f, float.MaxValue, delta),
        SmoothDamp(Camera.Position.Z, targetPosition.Z, ref _cameraVelocity.Z, 0.1f, float.MaxValue, delta)
    );

    Camera.LookAt(Position);
  }

  private float SmoothDamp(float current, float target, ref float currentVelocity, float smoothTime, float maxSpeed, float deltaTime) {
    smoothTime = Mathf.Max(0.0001f, smoothTime);
    var omega = 2f / smoothTime;

    var x = omega * deltaTime;
    var exp = 1f / (1f + x + (0.48f * x * x) + (0.235f * x * x * x));

    var change = current - target;
    var originalTo = target;

    var maxChange = maxSpeed * smoothTime;
    change = Mathf.Clamp(change, -maxChange, maxChange);
    target = current - change;

    var temp = (currentVelocity + omega * change) * deltaTime;
    currentVelocity = (currentVelocity - omega * temp) * exp;
    var output = target + (change + temp) * exp;

    if ((originalTo - current > 0f) == (output > originalTo)) {
      output = originalTo;
      currentVelocity = (output - originalTo) / deltaTime;
    }

    return output;
  }
}
