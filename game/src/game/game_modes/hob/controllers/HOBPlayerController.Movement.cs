namespace HOB;

using System.Threading.Tasks;
using Godot;

// Movement
public partial class HOBPlayerController {
  private bool _isPanning;
  private bool _isOrbiting;
  private Vector2 _lastMousePosition;

  private bool _exitingOrbit = false;
  private void MovementUpdate(float delta) {
    Character.Update(delta / Engine.TimeScale);

    Character.ClampPosition(GameBoard.GetAabb());
  }

  private async Task FocusOnSelectedEntity() {
    if (SelectedEntity != null) {
      await Character.MoveToPosition(SelectedEntity.GetPosition(), 1, Tween.TransitionType.Cubic);
      Character.SetZoom(1);
    }
  }

  private void NormalUnhandledInput(InputEvent @event) {
    if (!_gameStarted) {
      return;
    }

    if (@event.IsActionPressed(GameInputs.RotateRight)) {
      Character.RotateInDirection(true);
    }
    else if (@event.IsActionPressed(GameInputs.RotateLeft)) {
      Character.RotateInDirection(false);
    }

    if (@event.IsActionPressed(GameInputs.CameraPan)) {
      _isPanning = true;
      _lastMousePosition = GetViewport().GetMousePosition();
    }
    else if (@event.IsActionReleased(GameInputs.CameraPan)) {
      _isPanning = false;
    }

    if (Character.AllowZoom) {
      if (@event.IsActionPressed(GameInputs.ZoomIn)) {
        Character.AdjustZoom(1);
      }
      else if (@event.IsActionPressed(GameInputs.ZoomOut)) {
        Character.AdjustZoom(-1);
      }
    }

    if (!_isPanning && @event.IsActionPressed(GameInputs.Orbit)) {
      StateChart?.SendEvent("orbit");
    }
  }
  private void NormalMovementProcess(float delta) {
    delta /= (float)Engine.TimeScale;
    if (!_gameStarted) {
      return;
    }

    if (Input.IsActionPressed(GameInputs.SlowMoveSpeed)) {
      Character.SetMoveSpeedMulti(0.5f);
    }

    if (Input.IsActionPressed(GameInputs.FastMoveSpeed)) {
      Character.SetMoveSpeedMulti(2f);
    }

    if (!_isPanning) {
      Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
      var moveVector = Input.GetVector(GameInputs.MoveLeft, GameInputs.MoveRight, GameInputs.MoveForward, GameInputs.MoveBackward);
      if (moveVector != Vector2.Zero) {
        Character.HandleDirectionalMovement(delta, moveVector);
      }
      else {
        var mousePosition = GetViewport().GetMousePosition();
        var edgeMarginPixels = Character.EdgeMarginPixels;
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
          Character.HandleDirectionalMovement(delta, dir);
        }
        else {
          Character.Friction(delta);
        }
      }
    }
    else if (Character.AllowPan) {
      Input.SetDefaultCursorShape(Input.CursorShape.Drag);
      var currentMousePos = GetViewport().GetMousePosition();
      var displacement = currentMousePos - _lastMousePosition;
      _lastMousePosition = currentMousePos;

      // TODO: mouse wrap around screen when panning

      Character.Friction(delta);
      Character.HandlePanning(delta, displacement);
    }
  }

  private void OrbitStateEntered() {
    Input.MouseMode = Input.MouseModeEnum.Captured;

    Character.StartOrbit(SelectedEntity);
    _isOrbiting = true;
  }

  private void OrbitStateUnhandledInput(InputEvent @event) {
    if (!_exitingOrbit && @event.IsActionReleased(GameInputs.Orbit)) {
      //StateChart?.SendEvent("normal");
      _ = ResetOrbitCamera();
    }

    if (Character.AllowZoom) {
      if (@event.IsActionPressed(GameInputs.ZoomIn)) {
        Character.AdjustZoom(1);
      }
      else if (@event.IsActionPressed(GameInputs.ZoomOut)) {
        Character.AdjustZoom(-1);
      }
    }

    if (@event is InputEventMouseMotion mouseMotion) {
      Character.UpdateOrbit(mouseMotion.Relative, (float)GetProcessDeltaTime());
      GetViewport().SetInputAsHandled();
    }
  }

  private void OrbitStateProcess(float delta) {
    Character.Friction(delta);
  }

  private void OrbitStateExited() {
    Input.MouseMode = Input.MouseModeEnum.Visible;
    _isOrbiting = false;
  }

  private async Task ResetOrbitCamera() {
    _exitingOrbit = true;
    await Character.StopOrbit();
    StateChart?.SendEvent("normal");
    _exitingOrbit = false;
  }
}
