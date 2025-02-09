namespace HOB;

using GameplayFramework;
using Godot;

public partial class PlayerCharacter : Node3D, IPlayerControllable {
  [Export] public Camera3D Camera { get; private set; }
  [Export] public int EdgeMarginPixels { get; private set; } = 50;
  [Export] private float _acceleration = 10f;
  [Export] private float _moveSpeedMinZoom = 400, _moveSpeedMaxZoom = 100;
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

  public Vector3 Velocity { get; private set; }
  public float Zoom { get; private set; } = 1f;

  private float MoveSpeed => Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, Zoom);


  private float _targetZoom = 1f;
  private float _rotationAngle;

  public override void _Process(double delta) {
    Zoom = Mathf.Lerp(Zoom, _targetZoom, _zoomLerpSpeed * (float)delta);


    var distance = Mathf.Lerp(_minZoomOffset.Z, _maxZoomOffset.Z, _distanceCurve.Sample(Zoom));
    var height = Mathf.Lerp(_minZoomOffset.Y, _maxZoomOffset.Y, Zoom);
    Camera.Position = new(0, height, Mathf.Max(distance, 1));

    Camera.LookAt(Position);
  }

  public void AdjustZoom(float zoomDelta) {
    zoomDelta = Mathf.Clamp(zoomDelta, -1, 1);
    _targetZoom = Mathf.Clamp(_targetZoom + (zoomDelta * _zoomSpeed), 0, 1);
  }

  public void HandleDirectionalMovement(double delta, Vector2 horizontalDirection) {
    var direction = Transform.Basis * new Vector3(horizontalDirection.X, 0f, horizontalDirection.Y).Normalized();
    Velocity = Velocity.Lerp(direction * MoveSpeed, (float)delta * _acceleration);
  }

  public void HandlePanning(double delta, Vector2 mouseDisplacement) {
    Velocity += new Vector3(-mouseDisplacement.X, 0f, -mouseDisplacement.Y) * MoveSpeed * _panSpeedMulti * (float)delta;
  }

  public void MoveToPosition(Vector3 position) {
    var tween = CreateTween();
    tween.TweenProperty(this, "global_position", position, 3);
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
  }
  public T GetCharacter<T>() where T : Node => this as T;
}
