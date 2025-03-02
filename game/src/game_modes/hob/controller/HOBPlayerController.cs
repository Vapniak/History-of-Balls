namespace HOB;

using System;
using System.Linq;
using GameplayFramework;
using Godot;
using Godot.Collections;
using GodotStateCharts;
using HOB.GameEntity;
using RaycastSystem;

[GlobalClass]
public partial class HOBPlayerController : PlayerController, IMatchController {

  [Export] private Node StateChartNode { get; set; }
  [Export] private Array<HighlightColorMap> HighlightColors { get; set; }

  [Notify] public Entity SelectedEntity { get => _selectedEntity.Get(); private set => _selectedEntity.Set(value); }
  [Notify] public Command SelectedCommand { get => _selectedCommand.Get(); private set => _selectedCommand.Set(value); }
  [Notify] public GameCell HoveredCell { get => _hoveredCell.Get(); private set => _hoveredCell.Set(value); }

  public event Action EndTurnEvent;

  private StateChart StateChart { get; set; }

  private GameBoard GameBoard { get; set; }


  private PlayerCharacter _character;

  private bool _isPanning;
  private Vector2 _lastMousePosition;

  private HighlightSystem HighlightSystem { get; set; }

  public Team Team { get; set; }

  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;

    HighlightSystem = new(HighlightColors, GameBoard);

    GetHUD().EndTurnPressed += TryEndTurn;

    GameBoard.EntityAdded += (entity) => {

    };

    SelectedEntityChanged += OnSelectedEntityChanged;
    SelectedCommandChanged += UpdateCommandHighlights;

    _character = GetCharacter<PlayerCharacter>();

    _character.CenterPositionOn(GameBoard.GetAabb());

    GameInstance.GetGameState<IPauseGameState>().PausedEvent += () => GetHUD().Hide();
    GameInstance.GetGameState<IPauseGameState>().ResumedEvent += () => GetHUD().Show();

    StateChart = StateChart.Of(StateChartNode);
  }

  public override void _UnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(BuiltinInputActions.UICancel)) {
      if (!GetTree().Paused) {
        GetGameState().Pause();
      }
    }

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

  public void TryEndTurn() {
    if (IsCurrentTurn()) {
      EndTurnEvent?.Invoke();
    }
  }
  public void OwnTurnStarted() {
  }
  public void OwnTurnEnded() {
    SelectedCommand = null;
  }

  public void OnGameStarted() {
    CallDeferred(MethodName.SelectEntity, GameBoard.GetOwnedEntities(this)[0]);
    CallDeferred(MethodName.FocusOnSelectedEntity);

    GetHUD().OnGameStarted();
  }

  public void OnHUDCommandSelected(Command command) {
    SelectedCommand = command;
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

    var cell = GameBoard.Grid.GetCell(coord);

    if (cell != HoveredCell) {
      HoveredCell = cell;
    }

    var viewport = GetViewport();

    if (HoveredCell == null || _isPanning || viewport.GuiGetHoveredControl() != null) {
      GameBoard.SetMouseHighlight(false);
    }
    else {
      GameBoard.SetMouseHighlight(true);
    }
  }

  private void SelectEntity(Entity entity) {
    StateChart.SendEvent("entity_deselected");

    SelectedEntity = entity;

    if (IsInstanceValid(SelectedEntity)) {
      StateChart.SendEvent("entity_selected");
    }
  }

  private void CheckCommandInput(InputEvent @event) {
    // if (@event is InputEventKey eventKey) {
    //   if (@event.IsPressed()) {
    //     switch (eventKey.Keycode) {
    //       // TODO: for now its okay but later I want to make shortcuts for commands
    //       case Key.Key1:
    //         GetHUD().SelectCommand(0);
    //         break;
    //       case Key.Key2:
    //         GetHUD().SelectCommand(1);
    //         break;
    //       case Key.Key3:
    //         GetHUD().SelectCommand(2);
    //         break;
    //       case Key.Key4:
    //         GetHUD().SelectCommand(3);
    //         break;
    //       default:
    //         break;
    //     }
    //   }
    // }
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
  private void OnIdleStateEntered() {

  }

  private void OnIdleStateUnhandledInput(InputEvent @event) {
    if (@event.IsActionReleased(GameInputs.Select)) {
      var entites = GameBoard.GetEntitiesOnCell(HoveredCell);
      SelectEntity(entites.FirstOrDefault());
    }

    if (@event.IsActionPressed(GameInputs.EndTurn)) {
      TryEndTurn();
    }

    @event.Dispose();
  }

  private void OnSelectionStateEntered() {
    if (SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
      commandTrait.CommandStarted += OnCommandStarted;
      commandTrait.CommandFinished += OnCommandFinished;
    }

    SelectedEntity.TreeExiting += OnSelectedEntityDied;
    SelectedEntity.CellChanged += OnSelectedEntityCellChanged;
  }

  private void OnSelectionStateExited() {
    _character.CancelMoveToPosition();

    if (SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
      commandTrait.CommandStarted -= OnCommandStarted;
      commandTrait.CommandFinished -= OnCommandFinished;
    }

    SelectedEntity.TreeExiting -= OnSelectedEntityDied;
    SelectedEntity.CellChanged -= OnSelectedEntityCellChanged;

    SelectedCommand = null;
  }

  private void OnSelectionIdleStateUnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(GameInputs.Focus)) {
      FocusOnSelectedEntity();
    }

    CheckCommandInput(@event);


    if (@event.IsActionReleased(GameInputs.UseCommand)) {
      TryUseCommand(HoveredCell);
    }

    if (@event.IsActionReleased(GameInputs.Select)) {
      var entities = GameBoard.GetEntitiesOnCell(HoveredCell);

      if (entities.Contains(SelectedEntity)) {
        if (entities.Length > 1) {
          var index = System.Array.IndexOf(entities, SelectedEntity);
          index++;
          index %= entities.Length;
          SelectEntity(entities[index]);
        }
        else {
          SelectEntity(null);
        }
      }
      else {
        SelectEntity(entities.FirstOrDefault());
      }
    }

    if (@event.IsActionPressed(GameInputs.EndTurn)) {
      TryEndTurn();
    }

    @event.Dispose();
  }

  private void OnSelectionIdleStateEntered() {
    SelectFirstCommand();

    HoveredCellChanged += UpdateCommandHighlights;
  }

  private void OnSelectionIdleStateExited() {
    HoveredCellChanged -= UpdateCommandHighlights;
  }

  private void OnCommandStateEntered() {
    GetHUD().SetEndTurnButtonDisabled(true);
  }

  private void OnCommandStateExited() {
    GetHUD().SetEndTurnButtonDisabled(false);

    SelectedCommand = null;
  }
  #endregion

  private bool TryUseCommand(GameCell clickedCell) {
    if (!IsCurrentTurn() && SelectedCommand != null) {
      return false;
    }

    var entities = GameBoard.GetEntitiesOnCell(clickedCell);

    if (SelectedCommand is MoveCommand moveCommand) {
      if (moveCommand.GetReachableCells().Contains(clickedCell) && moveCommand.TryMove(this, clickedCell)) {
        return true;
      }
    }
    else if (SelectedCommand is AttackCommand attackCommand) {
      foreach (var entity in entities) {
        if (attackCommand.GetAttackableEntities().entities.Contains(entity) && attackCommand.TryAttack(this, entity)) {
          return true;
        }
      }
    }

    return false;
  }
  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;

  private void UpdateCommandHighlights() {
    if (SelectedEntity == null) {
      return;
    }

    HighlightSystem.ClearAllHighlights();

    HighlightSystem.SetHighlight(HighlightType.Selection, SelectedEntity.Cell);

    if (SelectedCommand != null) {
      var darkened = !SelectedCommand.GetEntity().TryGetOwner(out var owner) || owner != this || !SelectedCommand.CanBeUsed(this);

      if (SelectedCommand is MoveCommand moveCommand) {

        foreach (var cell in moveCommand.GetReachableCells()) {
          HighlightSystem.SetHighlight(HighlightType.Movement, cell, darkened);
        }

        if (HoveredCell != null && moveCommand.GetReachableCells().Contains(HoveredCell)) {
          foreach (var cell in moveCommand.FindPathTo(HoveredCell)) {
            HighlightSystem.SetHighlight(HighlightType.Path, cell, darkened);
          }
        }
      }
      else if (SelectedCommand is AttackCommand attackCommand) {
        var (entities, cellsInRange) = attackCommand.GetAttackableEntities();
        foreach (var cell in cellsInRange) {
          HighlightSystem.SetHighlight(HighlightType.Attack, cell, darkened);
        }

        foreach (var entity in entities) {
          HighlightSystem.SetHighlight(HighlightType.Attack, entity.Cell, darkened);
        }
      }
    }
    HighlightSystem.UpdateHighlights();
  }

  private void OnSelectedEntityCellChanged() {
    HighlightSystem.ClearAllHighlights();

    HighlightSystem.SetHighlight(HighlightType.Selection, SelectedEntity.Cell);

    HighlightSystem.UpdateHighlights();
  }

  private void OnSelectedEntityDied() {
    SelectEntity(null);
    HighlightSystem.ClearAllHighlights();
    HighlightSystem.UpdateHighlights();
  }


  private void OnSelectedEntityChanged() {
    HighlightSystem.ClearAllHighlights();

    if (SelectedEntity != null) {
      HighlightSystem.SetHighlight(HighlightType.Selection, SelectedEntity.Cell);
    }

    HighlightSystem.UpdateHighlights();
  }

  private void SelectFirstCommand() {
    if (SelectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
      if (commandTrait.Entity.TryGetOwner(out var owner) && owner == this) {
        SelectedCommand = commandTrait.GetCommands().FirstOrDefault(c => !c.UsedThisRound, defaultValue: null);
      }
      else {
        SelectedCommand = commandTrait.GetCommands().FirstOrDefault(defaultValue: null);
      }
    }
  }
}
