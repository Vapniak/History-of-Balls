namespace HOB;

using System;
using System.Linq;
using GameplayFramework;
using Godot;
using GodotStateCharts;
using HOB.GameEntity;
using RaycastSystem;

[GlobalClass]
public partial class HOBPlayerController : PlayerController, IMatchController {
  public event Action EndTurnEvent;

  [Export] private Node StateChartNode { get; set; }

  [ExportGroup("Highlight Types")]
  [Export] private HighlightType SelectedHighlightType { get; set; }
  [Export] private HighlightType MovableHighlightType { get; set; }
  [Export] private HighlightType AttackableHighlightType { get; set; }
  [Export] private HighlightType AttackHighlightType { get; set; }
  [Export] private HighlightType PathHighlightType { get; set; }

  // TODO: highlight material

  private StateChart StateChart { get; set; }

  private GameBoard GameBoard { get; set; }
  private Entity SelectedEntity { get; set; }
  private Command SelectedCommand { get; set; }
  private GameCell HoveredCell { get; set; }

  private Entity _lastSelectedEntity;
  private PlayerCharacter _character;

  private bool _isPanning;
  private Vector2 _lastMousePosition;


  public override void _Ready() {
    base._Ready();

    // TODO: unconfine mouse when in windowed mode
    //Input.MouseMode = Input.MouseModeEnum.Confined;

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

    @event.Dispose();
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;
  public override HOBHUD GetHUD() => base.GetHUD() as HOBHUD;

  public override void _Process(double delta) {
    _character.Move(delta);

    _character.ClampPosition(GetGameState().GameBoard.GetAabb());
  }

  // TODO: implement this inside interface and somehow call this inside this class
  public bool IsCurrentTurn() => GetGameState().IsCurrentTurn(this);
  public void OwnTurnStarted() {
    SelectEntity(SelectedEntity);
  }
  public void OwnTurnEnded() {
    SelectEntity(SelectedEntity);
  }

  public void OnGameStarted() {
    CallDeferred(MethodName.SelectEntity, GameBoard.GetOwnedEntities(this)[0]);
    CallDeferred(MethodName.FocusOnSelectedEntity);

    var playerState = GetPlayerState<HOBPlayerState>();

    GetHUD().UpdatePrimaryResourceName(playerState.PrimaryResourceType.Name);
    GetHUD().UpdateSecondaryResourceName(playerState.SecondaryResourceType.Name);

    GetHUD().UpdatePrimaryResourceValue(playerState.PrimaryResourceType.Value.ToString());
    GetHUD().UpdateSecondaryResourceValue(playerState.SecondaryResourceType.Value.ToString());

    playerState.PrimaryResourceType.ValueChanged += () => {
      GetHUD().UpdatePrimaryResourceValue(playerState.PrimaryResourceType.Value.ToString());
    };
    playerState.SecondaryResourceType.ValueChanged += () => {
      GetHUD().UpdateSecondaryResourceValue(playerState.SecondaryResourceType.Value.ToString());
    };
  }

  private void CheckHover() {
    var raycastResult = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.Entity | GameLayers.Physics3D.Mask.World);

    if (raycastResult == null) {
      HoveredCell = null;

      GameBoard.SetMouseHighlight(false);
      return;
    }

    var point = raycastResult.Position;
    var coord = GameBoard.Grid.GetLayout().PointToCube(new(point.X, point.Z));

    HoveredCell = GameBoard.Grid.GetCell(coord);


    if (HoveredCell == null || _isPanning) {
      GameBoard.SetMouseHighlight(false);
    }
    else {
      GameBoard.SetMouseHighlight(true);
    }
  }

  private void ShowCommandPanel() {
    if (!IsInstanceValid(SelectedEntity)) {
      return;
    }

    if (SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
      GetHUD().ShowCommandPanel(commandTrait);
    }
  }

  private void SelectEntity(Entity entity) {
    if (IsInstanceValid(SelectedEntity)) {
      SelectedEntity.CellChanged -= OnSelectedEntityCellChanged;
      SelectedEntity.TreeExited -= onSelectedEntityTreeExited;

      SelectedCommand = null;
    }

    if (!IsInstanceValid(entity)) {
      StateChart.SendEvent("entity_deselected");
      SelectedEntity = null;
      return;
    }

    SelectedEntity = entity;
    SelectedEntity.CellChanged += OnSelectedEntityCellChanged;
    SelectedEntity.TreeExited += onSelectedEntityTreeExited;

    void onSelectedEntityTreeExited() => SelectEntity(null);

    StateChart.SendEvent("entity_selected");
  }

  private void CheckCommandInput(InputEvent @event) {
    if (@event is InputEventKey eventKey) {
      if (@event.IsPressed()) {
        switch (eventKey.Keycode) {
          // TODO: for now its okay but later I want to make shortcuts for commands
          case Key.Key1:
            GetHUD().SelectCommand(0);
            break;
          case Key.Key2:
            GetHUD().SelectCommand(1);
            break;
          case Key.Key3:
            GetHUD().SelectCommand(2);
            break;
          case Key.Key4:
            GetHUD().SelectCommand(3);
            break;
          default:
            break;
        }
      }
    }
  }

  private void OnCommandSelected(Command command) {
    GameBoard.ClearHighlights();

    if (command != null) {
      if (command is MoveCommand moveCommand) {
        foreach (var cell in moveCommand.GetReachableCells()) {
          if (moveCommand.IsAvailable()) {
            GameBoard.SetHighlight(cell, MovableHighlightType);
          }
        }
      }
      else if (command is AttackCommand attackCommand) {
        var (entities, cellsInRange) = attackCommand.GetAttackableEntities();

        if (attackCommand.IsAvailable()) {
          foreach (var entity in entities) {
            GameBoard.SetHighlight(entity.Cell, AttackableHighlightType);
          }
          if (entities.Length == 0) {
            foreach (var cell in cellsInRange) {
              GameBoard.SetHighlight(cell, AttackHighlightType);
            }
          }
        }
        else {
          foreach (var cell in cellsInRange) {
            GameBoard.SetHighlight(cell, AttackHighlightType);
          }
        }
      }

    }

    GameBoard.SetHighlight(SelectedEntity.Cell, SelectedHighlightType);
    GameBoard.UpdateHighlights();
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

      // TODO: mouse wrap around screen when panning

      _character.Friction(delta);
      _character.HandlePanning(delta, displacement);
    }
  }

  #region  State Callbacks
  private void OnIdleEntered() {
    GameBoard.ClearHighlights();
    GameBoard.UpdateHighlights();
  }

  private void OnIdleUnhandledInput(InputEvent @event) {
    if (@event.IsActionReleased(GameInputs.Select)) {
      var entites = GameBoard.GetEntitiesOnCell(HoveredCell);
      SelectEntity(entites.FirstOrDefault());
    }

    @event.Dispose();
  }

  private void OnSelectionEntered() {
    GameBoard.ClearHighlights();

    if (SelectedEntity.IsOwnedBy(this)) {
      if (IsCurrentTurn() && SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
        commandTrait.CommandSelected += OnCommandSelected;
        commandTrait.CommandStarted += OnCommandStarted;
        commandTrait.CommandFinished += OnCommandFinished;
      }
    }

    GameBoard.UpdateHighlights();
  }

  private void OnSelectionExited() {
    _character.CancelMoveToPosition();

    GetHUD().HideStatPanel();
    GetHUD().HideCommandPanel();

    if (SelectedEntity.IsOwnedBy(this)) {
      if (SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
        commandTrait.CommandSelected -= OnCommandSelected;
        commandTrait.CommandStarted -= OnCommandStarted;
        commandTrait.CommandFinished -= OnCommandFinished;
      }
    }
  }

  private void OnSelectionIdleUnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(GameInputs.Focus)) {
      FocusOnSelectedEntity();
    }

    if (IsCurrentTurn()) {
      CheckCommandInput(@event);
    }


    if (SelectedCommand != null) {
      if (SelectedCommand is MoveCommand moveCommand) {
        if (HoveredCell != null) {
          GameBoard.ClearHighlights();
          GameBoard.SetHighlight(moveCommand.GetEntity().Cell, SelectedHighlightType);

          var reachable = moveCommand.GetReachableCells();
          foreach (var cell in reachable) {
            GameBoard.SetHighlight(cell, MovableHighlightType);
          }

          if (reachable.Contains(HoveredCell)) {
            foreach (var cell in moveCommand.FindPathTo(HoveredCell)) {
              GameBoard.SetHighlight(cell, PathHighlightType);
            }
          }

          GameBoard.UpdateHighlights();
        }
      }
    }

    if (@event.IsActionReleased(GameInputs.Select)) {
      var entities = GameBoard.GetEntitiesOnCell(HoveredCell);
      if (IsCurrentTurn()) {
        if (SelectedCommand != null) {
          if (SelectedCommand is MoveCommand moveCommand) {
            if (moveCommand.GetReachableCells().Contains(HoveredCell) && moveCommand.TryMove(HoveredCell)) {
              return;
            }
          }
          else if (SelectedCommand is AttackCommand attackCommand) {
            foreach (var entity in entities) {
              if (attackCommand.GetAttackableEntities().entities.Contains(entity) && attackCommand.TryAttack(entity)) {
                return;
              }
            }
          }
        }

        if (entities.FirstOrDefault() == SelectedEntity) {
          if (entities.Length > 1) {
            var index = Array.IndexOf(entities, SelectedEntity);
            index++;
            SelectEntity(entities[index]);
          }
          else {
            SelectEntity(null);
          }

          return;
        }
      }

      SelectEntity(entities.FirstOrDefault());
    }
    @event.Dispose();
  }

  private void OnSelectionIdleEntered() {
    GameBoard.ClearHighlights();

    GetHUD().ShowStatPanel(SelectedEntity);

    if (SelectedEntity.IsOwnedBy(this)) {
      GameBoard.SetHighlight(SelectedEntity.Cell, SelectedHighlightType);
      ShowCommandPanel();
    }
    else {
      GameBoard.SetHighlight(SelectedEntity.Cell, AttackableHighlightType);
    }

    GameBoard.UpdateHighlights();
  }

  private void OnSelectionIdleExited() {
    GetHUD().HideCommandPanel();
  }

  private void OnCommandEntered() {
    GameBoard.ClearHighlights();
    GameBoard.UpdateHighlights();

    GetHUD().SetEndTurnButtonDisabled(true);
  }

  private void OnCommandExited() {
    GetHUD().SetEndTurnButtonDisabled(false);
    SelectedCommand = null;
  }
  #endregion

  private void OnSelectedEntityCellChanged() {
    GameBoard.ClearHighlights();

    if (SelectedEntity.IsOwnedBy(this)) {
      GameBoard.SetHighlight(SelectedEntity.Cell, SelectedHighlightType);
    }
    else {
      GameBoard.SetHighlight(SelectedEntity.Cell, AttackableHighlightType);
    }

    GameBoard.UpdateHighlights();
  }
  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
}
