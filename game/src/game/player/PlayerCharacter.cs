namespace HOB;

using System.Threading.Tasks;
using GameplayFramework;
using Godot;

public partial class PlayerCharacter : Node3D, IPlayerControllable {
  [Export] public Camera3D Camera { get; private set; } = default!;
  [Export] public Node3D CameraParent { get; private set; } = default!;
  [Export] public Node3D Body { get; private set; } = default!;

  // Movement properties
  [Export] public int EdgeMarginPixels { get; private set; } = 50;
  [Export] private float _acceleration = 10f;
  [Export] private float _moveSpeedMinZoom = 400f, _moveSpeedMaxZoom = 100f;
  [Export] private float _friction = 10f;
  [Export] private float _stopSpeed = 5f;

  // Audio properties
  [ExportGroup("Audio")]
  [Export] private AudioStreamPlayer WindPlayer { get; set; } = default!;
  [Export] private float WindMinVolume { get; set; }
  [Export] private float WindMaxVolume { get; set; }

  // Zoom properties
  [ExportGroup("Zoom")]
  [Export(PropertyHint.Range, "0,1")] private float _targetZoom = 1f;
  [Export] public bool AllowZoom { get; private set; } = true;
  [Export] private float _zoomLerpSpeed = 5f;
  [Export(PropertyHint.Range, "0,1")] private float _zoomSpeed = 0.1f;
  [Export] private Vector3 _minZoomOffset = new(0, 300, 100);
  [Export] private Vector3 _maxZoomOffset = new(0, 40, 10);
  [Export] private Curve _distanceCurve;

  [ExportGroup("Panning")]
  [Export] public bool AllowPan { get; private set; } = true;
  [Export] private float _panSpeedMulti = 20f;

  [ExportGroup("Orbit Camera")]
  [Export] public Node3D? OrbitTarget { get; private set; }
  [Export] private float _orbitSpeed = 2f;

  // Orbit snap properties
  [ExportGroup("Orbit Snap")]
  [Export] public bool SnapToCardinalDirections = true;
  [Export] private float _snapDuration = 0.3f;
  [Export] private AudioStream _snapSound;

  // Public properties
  public Vector3 Velocity { get; private set; }
  public float Zoom { get; private set; } = 1f;
  public float MoveSpeed => Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, Zoom) * _moveSpeedMulti;

  // Private fields
  private float _moveSpeedMulti = 1;
  private float _zoomVelocity;
  private Vector3 _cameraVelocity;
  private Vector3 _panVelocity;
  private Tween? _snapTween;
  private bool _isMovingToPosition;
  private Tween? _moveTween;
  private Vector3 _prevPosition;
  private Vector3 _prevRotation;


  #region Orbit Camera Methods
  public void StartOrbit(Node3D? orbitTarget) {
    _snapTween?.Kill();

    OrbitTarget = orbitTarget;
  }

  public async Task StopOrbit() {
    if (SnapToCardinalDirections) {
      await SnapToNearest90Degrees();
    }
    else {
      OrbitTarget = null;
    }
  }

  public void UpdateOrbit(Vector2 rotationInput, float delta) {
    if (_snapTween != null && _snapTween.IsRunning()) {
      return;
    }

    Body.RotateY(Mathf.DegToRad(-rotationInput.X * _orbitSpeed));
  }

  private Tween? _rotationTween = null;
  public void RotateInDirection(bool right) {
    if (_rotationTween == null || !_rotationTween.IsRunning()) {
      _rotationTween = CreateTween().SetSpeedScale(1f / (float)Engine.TimeScale);

      _rotationTween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Back);
      _rotationTween.TweenProperty(Body, "rotation:y", Mathf.DegToRad(right ? 90 : -90), .5).AsRelative();
    }
  }

  private async Task SnapToNearest90Degrees() {
    // // Calculate nearest 90 degree yaw
    // float snappedYaw = Mathf.Round(_orbitYaw / 90f) * 90f;

    // // Play snap sound
    // //PlaySnapSound();

    // // Create tween for smooth snap
    // _snapTween = CreateTween();
    // _snapTween.SetParallel(true);

    // // Tween the yaw angle
    // _snapTween.TweenMethod(
    //     Callable.From<float>(value => {

    //     }),
    //     0f,
    //     1f,
    //     _snapDuration
    // ).SetEase(Tween.EaseType.Out);

    // await ToSignal(_snapTween, Tween.SignalName.Finished);
    // OrbitEnabled = false;
    // OrbitTarget = null;
    //_snapTween = null;
  }

  // private void PlaySnapSound() {
  //   if (_snapSound == null)
  //     return;

  //   var audioPlayer = new AudioStreamPlayer();
  //   audioPlayer.Stream = _snapSound;
  //   AddChild(audioPlayer);
  //   audioPlayer.Play();
  //   audioPlayer.Finished += () => audioPlayer.QueueFree();
  // }
  #endregion

  #region Core Movement Methods
  public void AdjustZoom(float zoomDelta) {
    zoomDelta = Mathf.Clamp(zoomDelta, -1, 1);
    _targetZoom = Mathf.Clamp(_targetZoom + (zoomDelta * _zoomSpeed), 0, 1);
  }

  public void SetZoom(float targetZoom) {
    targetZoom = Mathf.Clamp(targetZoom, 0, 1);
    _targetZoom = targetZoom;
  }

  public void HandleDirectionalMovement(double delta, Vector2 horizontalDirection) {
    if (_isMovingToPosition)
      return;

    var direction = new Vector3(horizontalDirection.X, 0f, horizontalDirection.Y).Normalized();
    Velocity = Velocity.Lerp(direction * MoveSpeed, (float)delta * _acceleration);
  }

  public void HandlePanning(double delta, Vector2 mouseDisplacement) {
    if (_isMovingToPosition)
      return;

    var targetVelocity = -new Vector3(mouseDisplacement.X, 0f, mouseDisplacement.Y) * MoveSpeed * _panSpeedMulti / (float)delta;
    Velocity = new Vector3(
        SmoothDamp(Velocity.X, targetVelocity.X, ref _panVelocity.X, 0.1f, float.MaxValue, (float)delta),
        Velocity.Y,
        SmoothDamp(Velocity.Z, targetVelocity.Z, ref _panVelocity.Z, 0.1f, float.MaxValue, (float)delta)
    );
  }

  public async Task MoveToPosition(Vector3 position, double duration, Tween.TransitionType transitionType) {
    _moveTween = CreateTween().SetSpeedScale(1f / (float)Engine.TimeScale);
    _moveTween.TweenProperty(this, "global_position", position, duration).SetTrans(transitionType);
    _isMovingToPosition = true;
    _moveTween.Finished += () => _isMovingToPosition = false;
    await ToSignal(_moveTween, Tween.SignalName.Finished);
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
    newSpeed = Mathf.Max(newSpeed, 0);

    if (vel.LengthSquared() > 0) {
      newSpeed /= vel.Length();
    }

    Velocity *= new Vector3(newSpeed, 1, newSpeed);
  }
  #endregion

  #region Update Methods
  public void Update(double delta) {
    UpdateCamera((float)delta);
    GlobalTranslate(Body.Transform.Basis * Velocity * (float)delta);

    var rotationSpeed = (Camera.GlobalRotation - _prevRotation).Length() / (float)delta;
    var speed = (Body.GlobalPosition - _prevPosition).Length() / (float)delta;
    speed += rotationSpeed;
    var targetVol = Mathf.Clamp(Mathf.Remap(speed, 0, MoveSpeed, WindMinVolume, WindMaxVolume),
        WindMinVolume, WindMaxVolume);
    WindPlayer.VolumeDb = (float)Mathf.Lerp(WindPlayer.VolumeDb, targetVol, delta * _zoomLerpSpeed);

    _prevPosition = Body.GlobalPosition;
    _prevRotation = Camera.GlobalRotation;
    _moveSpeedMulti = 1;
  }

  private void UpdateCamera(float delta) {
    Zoom = SmoothDamp(Zoom, _targetZoom, ref _zoomVelocity, 0.1f, float.MaxValue, delta);
    Camera.Fov = Mathf.Lerp(100, 80, Zoom);

    var targetDistance = Mathf.Lerp(_minZoomOffset.Z, _maxZoomOffset.Z, _distanceCurve.Sample(Zoom));
    var targetHeight = Mathf.Lerp(_minZoomOffset.Y, _maxZoomOffset.Y, Zoom);
    var targetPosition = new Vector3(0, targetHeight, Mathf.Max(targetDistance, 1));

    CameraParent.Position = new Vector3(
        SmoothDamp(CameraParent.Position.X, targetPosition.X, ref _cameraVelocity.X, 0.1f, float.MaxValue, delta),
        SmoothDamp(CameraParent.Position.Y, targetPosition.Y, ref _cameraVelocity.Y, 0.1f, float.MaxValue, delta),
        SmoothDamp(CameraParent.Position.Z, targetPosition.Z, ref _cameraVelocity.Z, 0.1f, float.MaxValue, delta)
    );

    Camera.LookAt(Position);
  }
  #endregion

  #region Utility Methods
  public void CenterPositionOn(Aabb aabb) {
    GlobalPosition = new(aabb.GetCenter().X, GlobalPosition.Y, aabb.GetCenter().Z);
  }

  public void ClampPosition(Aabb aabb) {
    var pos = GlobalPosition;
    pos = pos.Clamp(aabb.GetCenter() - (aabb.Size / 2), aabb.GetCenter() + (aabb.Size / 2));
    GlobalPosition = new(pos.X, GlobalPosition.Y, pos.Z);
  }

  public void SetMoveSpeedMulti(float value) => _moveSpeedMulti = value;

  public T GetCharacter<T>() where T : Node => this as T;

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
  #endregion
}