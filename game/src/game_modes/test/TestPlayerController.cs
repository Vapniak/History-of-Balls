namespace HOB;

using System;
using System.Collections.Generic;
using GameplayFramework;
using Godot;
using HOB.GameEntity;
using RaycastSystem;

[GlobalClass]
public partial class TestPlayerController : PlayerController, IMatchController {
  public event Action EndTurnEvent;

  private GameBoard GameBoard { get; set; }
  private Entity SelectedEntity { get; set; }
  private Command SelectedCommand { get; set; }


  private PlayerCharacter _character;

  private bool _isPanning;
  private Vector2 _lastMousePosition;


  public override void _Ready() {
    base._Ready();

    // TODO: unconfine mouse when in windowed mode
    Input.MouseMode = Input.MouseModeEnum.Confined;

    GameBoard = GetGameState().GameBoard;

    GetGameState().TurnChangedEvent += (playerIndex) => {
      GetHUD().OnTurnChanged(playerIndex);
    };

    GetGameState().RoundStartedEvent += (roundNumber) => {
      GetHUD().OnRoundChanged(roundNumber);
      ReselectEntity();
    };

    GetHUD().EndTurn += () => EndTurnEvent?.Invoke();


    _character = GetCharacter<PlayerCharacter>();

    _character.CenterPositionOn(GameBoard.GetAabb());
    GetHUD().HideCommandPanel();
    GetHUD().HideStatPanel();
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
      CheckSelection();
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


  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;
  public override TestHUD GetHUD() => base.GetHUD() as TestHUD;

  private void CheckSelection() {
    var raycastResult = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World);
    if (raycastResult == null) {
      return;
    }

    var point = raycastResult.Position;
    var coord = GameBoard.PointToCube(point);

    var cell = GameBoard.GetCell(coord);
    if (cell == null) {
      return;
    }

    CellClicked(cell);
  }

  private void CellClicked(GameCell cell) {
    var entities = GameBoard.GetEntitiesOnCell(cell);

    if (IsCurrentTurn() && SelectedCommand != null) {
      if (SelectedCommand is MoveCommand moveCommand) {
        if (moveCommand.TryMove(cell)) {
          ReselectEntity();
          return;
        }
      }
      else if (SelectedCommand is AttackCommand attackCommand) {
        if (entities.Length > 0) {
          if (attackCommand.TryAttack(entities[0])) {
            DeselectEntity();
          }
        }
        else {
          return;
        }
      }
    }


    if (entities.Length > 0) {
      if (entities[0] != SelectedEntity) {
        DeselectEntity();
        SelectEntity(entities[0]);
      }
    }
    else {
      DeselectEntity();
    }

    // GD.PrintS("Entities:", entities.Length, "Q:", cell.Coord.Q, "R:", cell.Coord.R);
  }

  // TODO: statemachine for selecting, entity move, attack
  private void SelectEntity(Entity entity) {
    GameBoard.ClearHighlights();

    SelectedEntity = entity;

    GetHUD().ShowStatPanel(SelectedEntity);


    if (SelectedEntity.IsOwnedBy(this)) {
      SelectedEntity.Cell.HighlightColor = Colors.White;
      if (IsCurrentTurn() && SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
        commandTrait.CommandSelected += OnCommandSelected;

        GetHUD().ShowCommandPanel(commandTrait);
      }
    }
    else {
      SelectedEntity.Cell.HighlightColor = Colors.Red;
    }

    GameBoard.UpdateHighlights();
  }

  private void DeselectEntity() {
    GetHUD().HideStatPanel();
    GetHUD().HideCommandPanel();

    // TODO: add events when entity is selected and deselected to listen in game board and do highlighting there
    GameBoard.ClearHighlights();
    // highlight units which you can select
    GameBoard.UpdateHighlights();

    if (SelectedEntity != null) {
      if (IsCurrentTurn() && SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
        commandTrait.CommandSelected -= OnCommandSelected;
      }
      SelectedEntity = null;
      SelectedCommand = null;
    }
  }

  private void ReselectEntity() {
    if (SelectedEntity == null) {
      return;
    }

    var entity = SelectedEntity;
    DeselectEntity();
    SelectEntity(entity);
  }

  private void OnCommandSelected(Command command) {
    if (command is MoveCommand moveCommand) {
      List<GameCell> availableCells = new();
      // TODO: add path finding
      foreach (var cell in GameBoard.GetCellsInRange(moveCommand.GetEntity().Cell.Coord, moveCommand.GetEntity().GetTrait<MoveTrait>().MovePoints)) {
        if (cell == moveCommand.GetEntity().Cell) {
          continue;
        }

        if (GameBoard.GetEntitiesOnCell(cell).Length == 0) {
          cell.HighlightColor = Colors.Green;
          availableCells.Add(cell);
        }
        else {
          cell.HighlightColor = Colors.Gray;
        }
      }

      moveCommand.GetEntity().GetTrait<MoveTrait>().CellsToMove = availableCells.ToArray();
    }
    else if (command is AttackCommand attackCommand) {
      List<Entity> attackable = new();
      foreach (var cell in GameBoard.GetCellsInRange(attackCommand.GetEntity().Cell.Coord, attackCommand.GetEntity().GetTrait<AttackTrait>().Range)) {
        if (cell == attackCommand.GetEntity().Cell) {
          continue;
        }

        var entites = GameBoard.GetEntitiesOnCell(cell);
        if (entites.Length == 0) {
          cell.HighlightColor = Colors.DarkRed;
        }
        else {
          if (entites[0].IsOwnedBy(this)) {
            cell.HighlightColor = Colors.Gray;
          }
          else {
            cell.HighlightColor = Colors.Red;
            attackable.Add(entites[0]);
          }
        }
      }

      attackCommand.AttackableEntities = attackable.ToArray();
    }

    GameBoard.UpdateHighlights();

    SelectedCommand = command;
  }

  // TODO: implement this inside interface and somehow call this inside this class
  public bool IsCurrentTurn() => GetGameState().IsCurrentTurn(this);
}
