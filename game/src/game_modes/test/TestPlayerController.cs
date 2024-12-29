namespace HOB;

using GameplayFramework;
using Godot;
using RaycastSystem;
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

  public override void _UnhandledInput(InputEvent @event) {
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
      Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
      var moveVector = Input.GetVector(GameInputs.MoveLeft, GameInputs.MoveRight, GameInputs.MoveForward, GameInputs.MoveBackward);
      if (moveVector != Vector2.Zero) {
        _character.HandleDirectionalMovement(delta, moveVector);
      }
      else {
        var mousePosition = GetViewport().GetMousePosition();
        var edgeMarginPixels = _character.EdgeMarginPixels;
        var screenRect = GetViewport().GetVisibleRect().Size;
        var dir = Vector2.Zero;

        if (mousePosition.X < edgeMarginPixels) {
          dir.X -= 1;
        }
        else if (mousePosition.X > screenRect.X - edgeMarginPixels) {
          dir.X += 1;
        }

        if (mousePosition.Y < edgeMarginPixels) {
          dir.Y -= 1;
        }
        else if (mousePosition.Y > screenRect.Y - edgeMarginPixels) {
          dir.Y += 1;
        }

        if (dir != Vector2.Zero) {
          Input.SetDefaultCursorShape(Input.CursorShape.Drag);
          _character.HandleDirectionalMovement(delta, dir);
        }
        else {
          _character.ApplyDrag(delta);
        }
      }
    }
    else if (_character.AllowPan) {
      // TODO: pan movement smoothing
      Input.SetDefaultCursorShape(Input.CursorShape.Drag);
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
    var raycastResult = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport());
    if (raycastResult != null) {
      var point = raycastResult.Position;
      var coordinates = Game.GetGameState<TestGameState>().GameBoard.Grid.Layout.PointToHexCoordinates(new(point.X, point.Z));
      GD.Print(coordinates.ToString());
    }
    else {
      GD.Print("No hit");
    }
  }
}
