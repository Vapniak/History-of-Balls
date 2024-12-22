namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;

public partial class PlayerCharacter : CharacterBody3D, IPlayerControllable {
  [Export] private float _zoomLerpSpeed = 5f;
  [Export] private Curve _distanceCurve;
  [Export(PropertyHint.Range, "0,1")] private float _zoomSpeed = 0.1f;
  [Export] private float _stickMinZoomDistance = 100, _stickMaxZoomDistance = 10;
  [Export] private float _stickMinZoomHeight = 300, _stickMaxZoomHeight = 40;
  [Export] private float _moveSpeedMinZoom = 400, _moveSpeedMaxZoom = 100;
  [Export] private Camera3D _camera;

  private float _zoom = 1f;
  private float _targetZoom = 1f;
  private float _rotationAngle;

  public override void _Process(double delta) {
    _zoom = Mathf.Lerp(_zoom, _targetZoom, _zoomLerpSpeed * (float)delta);


    var distance = Mathf.Lerp(_stickMinZoomDistance, _stickMaxZoomDistance, _distanceCurve.Sample(_zoom));
    var height = Mathf.Lerp(_stickMinZoomHeight, _stickMaxZoomHeight, _zoom);
    _camera.Position = new(0, height, Mathf.Max(distance, 1));

    _camera.LookAt(Position, UpDirection);
  }

  public void AdjustZoom(float zoomDelta) {
    _targetZoom = Mathf.Clamp(_targetZoom + (zoomDelta * _zoomSpeed), 0, 1);
  }

  public void AdjustPosition(double delta, float positionDeltaX, float positionDeltaZ) {
    var direction = Transform.Basis * new Vector3(positionDeltaX, 0f, positionDeltaZ).Normalized();
    var distance = Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, _zoom);
    Position += direction * distance * (float)delta;
  }

  public void ClampPosition(HexGrid hexGrid) {
    var pos = Position;
    pos.X = Mathf.Clamp(pos.X, 0f, hexGrid.GetRealSizeX());
    pos.Z = Mathf.Clamp(pos.Z, 0f, hexGrid.GetRealSizeZ());

    Position = new(pos.X, Position.Y, pos.Z);
  }
  public T GetCharacter<T>() where T : Node => this as T;
}
