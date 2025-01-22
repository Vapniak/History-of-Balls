namespace HOB;

using System;
using System.Collections.Generic;
using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;
using RaycastSystem;

[GlobalClass]
public partial class TestPlayerController : PlayerController, IMatchController {
  public event Action<CubeCoord> CoordClicked;

  private PlayerCharacter _character;

  private bool _isPanning;
  private Vector2 _lastMousePosition;


  public override void _Ready() {
    base._Ready();

    Input.MouseMode = Input.MouseModeEnum.Confined;

    _character = GetCharacter<PlayerCharacter>();

    _character.CenterPositionOn(GetGameState().GameBoard.GetAabb());
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



    // TODO: cooldown on selection because if someone has autoclicker I think it can crash game if you perform raycast every frame
    if (@event.IsActionPressed(GameInputs.Select)) {
      SelectCell();
    }


    // I saw a lot of objects created when I moved my mouse
    @event.Dispose();
  }

  public override void _Process(double delta) {
    base._Process(delta);

    _character.Move(delta);

    _character.ClampPosition(GetGameState().GameBoard.GetAabb());
  }

  public override void _PhysicsProcess(double delta) {
    base._PhysicsProcess(delta);

    // TODO: maybe make it more readable?
    // TODO: better movement, not using lerps
    if (!_isPanning) {
      GetGameState().GameBoard.SetMouseHighlight(true);
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
          _character.Friction(delta);
        }
      }
    }
    else if (_character.AllowPan) {
      Input.SetDefaultCursorShape(Input.CursorShape.Drag);
      var currentMousePos = GetViewport().GetMousePosition();
      var displacement = currentMousePos - _lastMousePosition;
      _lastMousePosition = currentMousePos;

      GetGameState().GameBoard.SetMouseHighlight(false);

      // TODO: mouse wrap around screen when panning

      _character.Friction(delta);
      _character.HandlePanning(delta, displacement);
    }
  }

  // TODO: make controller not game state compatibile but game mode
  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  private void SelectCell() {
    var raycastResult = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World);
    if (raycastResult != null) {
      var point = raycastResult.Position;
      var coord = GetGameState().GameBoard.PointToCube(point);
      CoordClicked?.Invoke(coord);
    }
    else {
      //GD.Print("No hit");
    }
  }
}
