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
  [Notify] public GameCell HoveredCell { get => _hoveredCell.Get(); private set => _hoveredCell.Set(value); }

  public event Action EndTurnEvent;
  public Country Country { get; set; }

  private StateChart StateChart { get; set; }

  private GameBoard GameBoard => GetGameState().GameBoard;
  private PlayerCharacter Character => GetCharacter<PlayerCharacter>();

  private bool _isPanning;
  private Vector2 _lastMousePosition;

  private HighlightSystem HighlightSystem { get; set; }
  private IEntityManagment EntityManagment => GetGameMode().GetEntityManagment();

  private string _entityUISceneUID = "uid://omhtmi8gorif";

  public override void _Ready() {
    base._Ready();

    HighlightSystem = new(HighlightColors, GameBoard);

    GetHUD().EndTurnPressed += TryEndTurn;

    SelectedEntityChanged += OnSelectedEntityChanged;

    Character.CenterPositionOn(GameBoard.GetAabb());

    StateChart = StateChart.Of(StateChartNode);

    GetGameMode().GetEntityManagment().EntityAdded += onEntityAdded;
  }

  void onEntityAdded(Entity entity) {

  }

  public override void _Notification(int what) {
    base._Notification(what);

    if (what == MainLoop.NotificationApplicationFocusOut) {
      GetGameMode().Pause();
    }
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);

    if (@event.IsActionPressed(BuiltinInputActions.UICancel)) {
      if (!GetTree().Paused) {
        GetViewport().SetInputAsHandled();
        GetGameMode().Pause();
      }
    }
  }

  public override void _UnhandledInput(InputEvent @event) {
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

    @event.Dispose();
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;
  public override HOBHUD GetHUD() => base.GetHUD() as HOBHUD;

  public override void _Process(double delta) {
    Character.Move(delta);

    Character.ClampPosition(GameBoard.GetAabb());
  }

  public void TryEndTurn() {
    if (((IMatchController)this).IsCurrentTurn()) {
      EndTurnEvent?.Invoke();
    }
  }
  public void OwnTurnStarted() {

  }
  public void OwnTurnEnded() {

  }

  public void OnGameStarted() {
    CallDeferred(MethodName.SelectEntity, EntityManagment.GetOwnedEntites(this)[0]);
    CallDeferred(MethodName.FocusOnSelectedEntity);

    GetHUD().OnGameStarted();
  }

  private void CheckHover() {
    var raycastResult = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World);

    if (raycastResult == null) {
      HoveredCell = null;

      GameBoard.SetMouseHighlight(false);
      return;
    }

    var point = raycastResult.Position;
    var coord = GameBoard.Grid.GetLayout().PointToCube(new(point.X, point.Z));


    // DebugDraw3D.DrawSphere(point);

    var cell = GameBoard.Grid.GetCell(coord);

    if (cell != HoveredCell) {
      HoveredCell = cell;
    }

    var viewport = GetViewport();

    if (HoveredCell == null || _isPanning || (viewport.GuiGetHoveredControl() != null && viewport.GuiGetHoveredControl().GetParent() is Control)) {
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

  private void FocusOnSelectedEntity() {
    if (SelectedEntity != null) {
      Character.MoveToPosition(SelectedEntity.GetPosition(), 1, Tween.TransitionType.Cubic);
    }
  }
  private void HandleMovement(float delta) {
    if (Input.IsActionPressed(GameInputs.SpeedMulti)) {
      Character.ApplySpeedMulti();
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

  #region  State Callbacks
  private void OnIdleStateEntered() {

  }

  private void OnIdleStateUnhandledInput(InputEvent @event) {
    if (@event.IsActionReleased(GameInputs.Select)) {
      var entites = EntityManagment.GetEntitiesOnCell(HoveredCell);
      SelectEntity(entites.FirstOrDefault());
    }

    if (@event.IsActionPressed(GameInputs.EndTurn)) {
      TryEndTurn();
    }

    @event.Dispose();
  }

  private void OnSelectionStateEntered() {
    SelectedEntity.TreeExiting += OnSelectedEntityDied;
    SelectedEntity.CellChanged += OnSelectedEntityCellChanged;

    SelectedEntity.AbilitySystem.TryActivateAbility<MoveAbility>(null);
  }

  private void OnSelectionStateExited() {
    Character.CancelMoveToPosition();

    SelectedEntity.TreeExiting -= OnSelectedEntityDied;
    SelectedEntity.CellChanged -= OnSelectedEntityCellChanged;
  }

  private void OnSelectionIdleStateUnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(GameInputs.Focus)) {
      FocusOnSelectedEntity();
    }

    CheckCommandInput(@event);


    if (@event.IsActionPressed(GameInputs.UseCommand)) {
      if (SelectedEntity.AbilitySystem.TryGetAbility<MoveAbility>(out var moveAbility)) {
        moveAbility.SelectCellToMove(HoveredCell);
        return;
      }
    }

    if (@event.IsActionReleased(GameInputs.Select)) {
      var entities = EntityManagment.GetEntitiesOnCell(HoveredCell);

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

  }

  private void OnSelectionIdleStateExited() {

  }

  private void OnCommandStateEntered() {
    GetHUD().SetEndTurnButtonDisabled(true);
  }

  private void OnCommandStateExited() {
    GetHUD().SetEndTurnButtonDisabled(false);
  }
  #endregion


  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;

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

  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;
}
