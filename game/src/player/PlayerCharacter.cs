namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;

public partial class PlayerCharacter : CharacterBody3D, IPlayerControllable {
  [Export] private float _stickMinZoom = -250, _stickMaxZoom = -45;
  [Export] private float _swivelMinZoomAngle = 90, _swivelMaxZoomAngle = 45;
  [Export] private float _moveSpeedMinZoom = 400, _moveSpeedMaxZoom = 100;
  [Export] private float _rotationSpeed = 180;
  [Export] private Node3D _swivel, _stick;

  private float _zoom = 1f;
  private float _rotationAngle;

  public void AdjustZoom(float zoomDelta) {
    _zoom = Mathf.Clamp(_zoom + zoomDelta, 0, 1);
    var distance = Mathf.Lerp(_stickMinZoom, _stickMaxZoom, _zoom);
    _stick.Position = new(0, distance, 0);

    var angle = Mathf.Lerp(_swivelMinZoomAngle, _swivelMaxZoomAngle, _zoom);
    _swivel.Rotation = new(Mathf.DegToRad(angle), 0f, 0f);
  }

  public void AdjustRotation(double delta, float rotationDelta) {
    _rotationAngle += rotationDelta * _rotationSpeed * (float)delta;
    _rotationAngle = Mathf.Wrap(_rotationAngle, -360, 360);
    Rotation = new(0f, _rotationAngle, 0f);
  }

  public void AdjustPosition(double delta, Vector3 positionDelta) {
    var direction = Transform.Basis.Z * new Vector3(positionDelta.X, 0f, positionDelta.Z).Normalized();
    var damping = Mathf.Max(Mathf.Abs(positionDelta.X), Mathf.Abs(positionDelta.Z));
    var distance = Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, _zoom) * damping * (float)delta;
    Position += direction * distance;
  }

  public void ClampPosition(HexGrid hexGrid) {
    var pos = Position;
    pos.X = Mathf.Clamp(pos.X, 0f, hexGrid.GetRealSizeX());
    pos.Z = Mathf.Clamp(pos.Z, 0f, hexGrid.GetRealSizeZ());

    Position = new(pos.X, Position.Y, pos.Z);
  }
  public T GetCharacter<T>() where T : Node => this as T;
}
