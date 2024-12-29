namespace HOB;

using System;
using GameplayFramework;
using Godot;
using HexGridMap;

public partial class PlayerCharacter : Node3D, IPlayerControllable {
  [Export] public Camera3D Camera { get; private set; }
  [Export] private float _acceleration = 10f;
  [Export] private float _moveSpeedMinZoom = 400, _moveSpeedMaxZoom = 100;
  [Export] private int _edgeMarginPixels = 50;

  [ExportGroup("Zoom")]
  [Export] public bool AllowZoom { get; private set; } = true;
  [Export] private float _zoomLerpSpeed = 5f;
  [Export(PropertyHint.Range, "0,1")] private float _zoomSpeed = 0.1f;
  [Export] private Vector3 _minZoomOffset = new(0, 300, 100);
  [Export] private Vector3 _maxZoomOffset = new(0, 40, 10);
  [Export] private Curve _distanceCurve;


  [ExportGroup("Panning")]
  [Export] public bool AllowPan { get; private set; } = true;

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
    var panSpeedMulti = .2f;
    Velocity = Vector3.Zero;
    GlobalTranslate(new Vector3(-mouseDisplacement.X, 0f, -mouseDisplacement.Y) * (float)delta * MoveSpeed * panSpeedMulti);
  }

  public void HandleEdgeMovement(double delta, Vector2 mousePosition, Vector2 screenRect) {
    var dir = Vector2.Zero;

    if (mousePosition.X < _edgeMarginPixels) {
      dir.X -= 1;
    }
    else if (mousePosition.X > screenRect.X - _edgeMarginPixels) {
      dir.X += 1;
    }

    if (mousePosition.Y < _edgeMarginPixels) {
      dir.Y -= 1;
    }
    else if (mousePosition.Y > screenRect.Y - _edgeMarginPixels) {
      dir.Y += 1;
    }

    HandleDirectionalMovement(delta, dir);
  }

  // TODO: position clamping to grid size
  // public void ClampPosition(HexGrid hexGrid) {
  //   var pos = Position;
  //   pos.X = Mathf.Clamp(pos.X, 0f, hexGrid.GetRealSizeX());
  //   pos.Z = Mathf.Clamp(pos.Z, 0f, hexGrid.GetRealSizeZ());

  //   Position = new(pos.X, Position.Y, pos.Z);
  // }

  public void Move(double delta) {
    GlobalTranslate(Velocity * (float)delta);
  }
  public T GetCharacter<T>() where T : Node => this as T;
}
