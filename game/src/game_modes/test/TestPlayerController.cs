namespace HOB;

using GameplayFramework;
using Godot;
using System;

[GlobalClass]
public partial class TestPlayerController : PlayerController {

  private PlayerCharacter _character;

  private bool _isPanning;
  private Vector2 _lastMousePosition;
  public override void _Ready() {
    base._Ready();

    _character = GetCharacter<PlayerCharacter>();
  }

  public override void _Input(InputEvent @event) {
    if (@event.IsActionPressed(GameInputs.CameraPan)) {
      _isPanning = true;
      _lastMousePosition = GetViewport().GetMousePosition();
    }
    else if (@event.IsActionReleased(GameInputs.CameraPan)) {
      _isPanning = false;
    }

    if (_character.AllowZoom) {
      if (@event.IsActionPressed(GameInputs.ZoomIn)) {
        _character.AdjustZoom(1);
      }
      else if (@event.IsActionPressed(GameInputs.ZoomOut)) {
        _character.AdjustZoom(-1);
      }
    }

    if (@event.IsActionPressed(GameInputs.Select)) {
      SelectCell();
    }
  }

  public override void _Process(double delta) {
    base._Process(delta);

    if (!_isPanning) {
      var moveVector = Input.GetVector(GameInputs.MoveLeft, GameInputs.MoveRight, GameInputs.MoveForward, GameInputs.MoveBackward);
      if (moveVector != Vector2.Zero) {
        _character.HandleDirectionalMovement(delta, moveVector);
      }
      else {
        _character.HandleEdgeMovement(delta, GetViewport().GetMousePosition(), GetViewport().GetVisibleRect().Size);
      }
    }
    else if (_character.AllowPan) {
      var currentMousePos = GetViewport().GetMousePosition();
      var displacement = currentMousePos - _lastMousePosition;
      _lastMousePosition = currentMousePos;

      _character.HandlePanning(delta, displacement);
    }

    _character.Move(delta);


    // TODO: position clamping
    // _character.ClampPosition(Game.GetGameState<TestGameState>().HexGrid);
  }

  private void SelectCell() {
    var mousePos = GetViewport().GetMousePosition();
    var from = _character.Camera.ProjectRayOrigin(mousePos);
    var to = from + (_character.Camera.ProjectRayNormal(mousePos) * 1000);
    var rayQuery = PhysicsRayQueryParameters3D.Create(from, to);
    var space = GetWorld3D().DirectSpaceState;
    var raycastResult = space.IntersectRay(rayQuery);
    if (raycastResult.Count > 0) {
      var point = raycastResult["position"].AsVector3();
      var coordinates = Game.GetGameState<TestGameState>().GameBoard.Grid.Layout.PointToHexCoordinates(new(point.X, point.Z));
      GD.Print(coordinates.ToString());
    }
    else {
      GD.Print("No hit");
    }
  }
}
