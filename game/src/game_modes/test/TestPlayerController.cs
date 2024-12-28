namespace HOB;

using GameplayFramework;
using Godot;
using System;

[GlobalClass]
public partial class TestPlayerController : PlayerController {

  private PlayerCharacter _character;
  public override void _Ready() {
    base._Ready();

    _character = GetCharacter<PlayerCharacter>();
  }

  // TODO: move it into physics process and add acceleration, also add movement on moving mouse to screen corners
  public override void _Process(double delta) {
    base._Process(delta);

    if (Input.IsActionJustPressed(GameInputs.Select)) {
      var mousePos = GetViewport().GetMousePosition();
      var from = _character.Camera.ProjectRayOrigin(mousePos);
      var to = from + (_character.Camera.ProjectRayNormal(mousePos) * 1000);
      var rayQuery = PhysicsRayQueryParameters3D.Create(from, to);
      var space = GetWorld3D().DirectSpaceState;
      var raycastResult = space.IntersectRay(rayQuery);
      if (raycastResult.Count > 0) {
        GD.Print(raycastResult);
        var point = raycastResult["position"].AsVector3();
        var coordinates = Game.GetGameState<TestGameState>().GameBoard.Grid.Layout.PointToHexCoordinates(new(point.X, point.Z));
        GD.Print(coordinates.ToString());
      }
      else {
        GD.Print("No hit");
      }
    }

    float zoomDelta = 0;
    if (Input.IsActionJustPressed(GameInputs.ZoomIn)) {
      zoomDelta = 1;
    }
    else if (Input.IsActionJustPressed(GameInputs.ZoomOut)) {
      zoomDelta = -1;
    }

    if (zoomDelta != 0) {
      _character.AdjustZoom(zoomDelta);
    }

    var moveVector = Input.GetVector(GameInputs.MoveLeft, GameInputs.MoveRight, GameInputs.MoveForward, GameInputs.MoveBackward);
    _character.Accelerate(delta, moveVector.X, moveVector.Y);
    _character.Move(delta);
    // _character.ClampPosition(Game.GetGameState<TestGameState>().HexGrid);
  }
}
