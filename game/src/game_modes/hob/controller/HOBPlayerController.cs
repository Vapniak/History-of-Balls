namespace HOB;

using System;
using GameplayFramework;
using Godot;
using GodotStateCharts;
using HOB.GameEntity;
using RaycastSystem;

[GlobalClass]
public partial class HOBPlayerController : PlayerController, IMatchController {
  public event Action EndTurnEvent;

  [Export] private Node StateChartNode { get; set; }
  private StateChart StateChart { get; set; }

  private GameBoard GameBoard { get; set; }
  private Entity SelectedEntity { get; set; }
  private Command SelectedCommand { get; set; }
  private GameCell HoveredCell { get; set; }


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
    };

    GetHUD().EndTurn += () => EndTurnEvent?.Invoke();


    _character = GetCharacter<PlayerCharacter>();

    _character.CenterPositionOn(GameBoard.GetAabb());

    GetHUD().HideCommandPanel();
    GetHUD().HideStatPanel();
    GetHUD().HideHoverStatPanel();

    GameBoard.SetMouseHighlight(true);

    StateChart = StateChart.Of(StateChartNode);
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

    // I saw a lot of objects created when I moved my mouse
    @event.Dispose();
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;
  public override HOBHUD GetHUD() => base.GetHUD() as HOBHUD;

  public override void _Process(double delta) {
    _character.Move(delta);

    _character.ClampPosition(GetGameState().GameBoard.GetAabb());
  }

  public override void _PhysicsProcess(double delta) {
    base._PhysicsProcess(delta);

    CheckHover();
  }

  // TODO: implement this inside interface and somehow call this inside this class
  public bool IsCurrentTurn() => GetGameState().IsCurrentTurn(this);
  public void OwnTurnStarted() {
    ReselectEntity();
  }
  public void OwnTurnEnded() { }

  public void OnGameStarted() {
    CallDeferred(MethodName.SelectEntity, GameBoard.GetOwnedEntities(this)[0]);
    CallDeferred(MethodName.FocusOnSelectedEntity);

    var playerState = GetPlayerState<HOBPlayerState>();

    GetHUD().UpdatePrimaryResourceName(playerState.PrimaryResourceType.Name);
    GetHUD().UpdateSecondaryResourceName(playerState.SecondaryResourceType.Name);

    GetHUD().UpdatePrimaryResourceValue(playerState.PrimaryResourceType.Value.ToString());
    GetHUD().UpdateSecondaryResourceValue(playerState.SecondaryResourceType.Value.ToString());

    playerState.PrimaryResourceType.ValueChanged += () => GetHUD().UpdatePrimaryResourceValue(playerState.PrimaryResourceType.Value.ToString());
    playerState.SecondaryResourceType.ValueChanged += () => GetHUD().UpdateSecondaryResourceValue(playerState.SecondaryResourceType.Value.ToString());
  }

  private GameCell CheckSelection() {
    var raycastResult = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World);
    if (raycastResult == null) {
      return null;
    }

    var point = raycastResult.Position;
    var coord = GameBoard.PointToCube(point);

    var cell = GameBoard.GetCell(coord);

    return cell;
  }

  private void CheckHover() {
    var raycastResult = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World);
    if (raycastResult == null) {
      return;
    }

    var point = raycastResult.Position;
    var coord = GameBoard.PointToCube(point);

    var cell = GameBoard.GetCell(coord);

    var entities = GameBoard.GetEntitiesOnCell(cell);
    if (cell == null || _isPanning || entities.Length == 0) {
      UnHoverCell();
      return;
    }


    if (HoveredCell != cell) {
      HoverCell(cell);
    }
  }

  private void HoverCell(GameCell cell) {
    HoveredCell = cell;

    var entities = GameBoard.GetEntitiesOnCell(cell);

    if (entities.Length > 0) {
      GetHUD().ShowHoverStatPanel(entities[0]);
    }
  }

  private void UnHoverCell() {
    GetHUD().HideHoverStatPanel();

    HoveredCell = null;
  }

  private void SelectEntity(Entity entity) {
    SelectedEntity = entity;

    StateChart.SendEvent("entity_selected");
  }

  private void DeselectEntity() {
    StateChart.SendEvent("entity_deselected");
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
    GameBoard.ClearHighlights();


    if (command is MoveCommand moveCommand) {
      foreach (var cell in moveCommand.EntityMoveTrait.GetReachableCells()) {
        GameBoard.SetHighlight(cell, Colors.Green);
      }
    }
    else if (command is AttackCommand attackCommand) {
      var (entities, cellsInRange) = attackCommand.EntityAttackTrait.GetAttackableEntities(GameBoard);
      foreach (var cell in cellsInRange) {
        GameBoard.SetHighlight(cell, Colors.DarkRed);
      }
      foreach (var entity in entities) {
        GameBoard.SetHighlight(entity.Cell, Colors.Red);
      }
    }

    GameBoard.SetHighlight(command.GetEntity().Cell, Colors.White);

    SelectedCommand = command;
  }

  private void OnCommandStarted(Command command) {
    StateChart.SendEvent("command_started");
  }

  private void OnCommandFinished(Command command) {
    StateChart.SendEvent("command_finished");
  }

  private void FocusOnSelectedEntity() {
    if (SelectedEntity != null) {
      _character.MoveToPosition(SelectedEntity.GetPosition(), 1, Tween.TransitionType.Cubic);
    }
  }
  private void HandleMovement(float delta) {
    if (Input.IsActionPressed(GameInputs.SpeedMulti)) {
      _character.ApplySpeedMulti();
    }

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

  private void OnIdleUnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(GameInputs.Select)) {
      var cell = CheckSelection();

      if (cell == null) {
        return;
      }

      var entities = GameBoard.GetEntitiesOnCell(cell);
      if (entities.Length > 0) {
        if (entities[0] != SelectedEntity) {
          SelectEntity(entities[0]);
        }
      }
    }

    @event.Dispose();
  }

  private void OnSelectionEntered() {
    GameBoard.ClearHighlights();

    GetHUD().ShowStatPanel(SelectedEntity);

    if (SelectedEntity.IsOwnedBy(this)) {
      if (IsCurrentTurn() && SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
        commandTrait.CommandSelected += OnCommandSelected;
        commandTrait.CommandStarted += OnCommandStarted;
        commandTrait.CommandFinished += OnCommandFinished;

        GetHUD().ShowCommandPanel(commandTrait);
      }
    }

    GameBoard.UpdateHighlights();
  }

  private void OnSelectionExited() {
    _character.CancelMoveToPosition();

    GetHUD().HideStatPanel();
    GetHUD().HideCommandPanel();

    if (SelectedEntity != null) {
      if (SelectedEntity.IsOwnedBy(this)) {
        if (SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
          commandTrait.CommandSelected -= OnCommandSelected;
          commandTrait.CommandStarted -= OnCommandStarted;
          commandTrait.CommandFinished -= OnCommandFinished;
        }

        SelectedEntity = null;
        SelectedCommand = null;
      }
    }
  }

  private void OnSelectionIdleUnhandledInput(InputEvent @event) {

    if (@event.IsActionPressed(GameInputs.Focus)) {
      FocusOnSelectedEntity();
    }

    if (@event.IsActionPressed(GameInputs.Select)) {
      var cell = CheckSelection();
      if (cell == null) {
        return;
      }

      var entities = GameBoard.GetEntitiesOnCell(cell);
      if (IsCurrentTurn()) {
        if (SelectedEntity != null && entities.Length > 0) {
          if (SelectedEntity == entities[0]) {
            DeselectEntity();
            return;
          }
        }

        if (SelectedCommand != null) {
          if (SelectedCommand is MoveCommand moveCommand) {
            if (entities.Length == 0) {
              if (moveCommand.TryMove(cell)) {
                return;
              }
            }
          }
          else if (SelectedCommand is AttackCommand attackCommand) {
            if (entities.Length > 0) {
              if (attackCommand.TryAttack(entities[0])) {
                return;
              }
            }
          }
        }
      }

      DeselectEntity();
      if (entities.Length > 0) {
        SelectEntity(entities[0]);
      }
    }

    @event.Dispose();
  }

  private void OnSelectionIdleEntered() {
    if (SelectedEntity.IsOwnedBy(this)) {
      GameBoard.SetHighlight(SelectedEntity.Cell, Colors.White);
    }
    else {
      GameBoard.SetHighlight(SelectedEntity.Cell, Colors.Red);
    }

    GameBoard.UpdateHighlights();
  }

  private void OnSelectionIdleExited() {

  }

  private void OnCommandEntered() {
    GameBoard.ClearHighlights();
    GameBoard.UpdateHighlights();
  }

  private void OnCommandExited() {
    GetHUD().ShowCommandPanel(SelectedCommand.CommandTrait);
  }
}
