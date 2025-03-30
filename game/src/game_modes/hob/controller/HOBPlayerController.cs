namespace HOB;

using System;
using System.Diagnostics;
using System.Linq;
using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;
using GodotStateCharts;
using HOB.GameEntity;
using RaycastSystem;

[GlobalClass]
public partial class HOBPlayerController : PlayerController, IMatchController {
  [Export] private Node? StateChartNode { get; set; }
  [Export] private Array<HighlightColorMap>? HighlightColors { get; set; }

  [Notify] public Entity? SelectedEntity { get => _selectedEntity.Get(); private set => _selectedEntity.Set(value); }
  [Notify] public GameCell? HoveredCell { get => _hoveredCell.Get(); private set => _hoveredCell.Set(value); }

  [Notify] public HOBAbilityInstance? SelectedCommand { get => _selectedCommand.Get(); private set => _selectedCommand.Set(value); }

  public event Action? EndTurnEvent;

  private StateChart? StateChart { get; set; }

  private GameBoard GameBoard => GetGameState().GameBoard;
  private PlayerCharacter Character => GetCharacter<PlayerCharacter>();

  private bool _isPanning;
  private Vector2 _lastMousePosition;

  private HighlightSystem? HighlightSystem { get; set; }
  private IEntityManagment EntityManagment => GetGameMode().GetEntityManagment();

  private string _entityUISceneUID = "uid://omhtmi8gorif";

  private Dictionary<Entity, EntityUI> EntityUIs { get; set; } = new();

  public override void _Ready() {
    base._Ready();

    HighlightSystem = new(HighlightColors ?? new(), GameBoard);

    GetHUD().EndTurnPressed += TryEndTurn;

    SelectedEntityChanged += OnSelectedEntityChanged;
    SelectedCommandChanged += OnSelectedCommandChanged;

    Character.CenterPositionOn(GameBoard.GetAabb());

    if (StateChartNode != null) {
      StateChart = StateChart.Of(StateChartNode);
    }

    GetGameMode().GetEntityManagment().EntityAdded += OnEntityAdded;

    GetHUD().CommandSelected += (command) => {
      SelectedCommand = command;
      OnCommandSelected();
    };
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

    foreach (var entity in EntityManagment.GetEntities()) {
      SpawnUIForEntity(entity);
    }
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

  private void SelectEntity(Entity? entity) {
    StateChart?.SendEvent("entity_deselected");

    SelectedEntity = entity;

    if (IsInstanceValid(SelectedEntity)) {
      StateChart?.SendEvent("entity_selected");
    }
  }

  private void CheckCommandInput(InputEvent @event) {
    if (@event.IsActionPressed(GameInputs.UseCommand)) {
      if (SelectedEntity == null || SelectedCommand == null || HoveredCell == null) {
        return;
      }

      GameplayEventData? eventData = null;
      if (SelectedCommand is MoveAbilityResource.Instance moveAbility) {
        eventData = new() {
          Activator = this,
          TargetData = new MoveTargetData() { Cell = HoveredCell }
        };
      }
      else if (SelectedCommand is AttackAbilityResource.Instance attackAbility) {
        var unit = EntityManagment.GetEntitiesOnCell(HoveredCell)?.FirstOrDefault(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeUnit)), null);
        if (unit != null) {
          eventData = new() { Activator = this, TargetData = new AttackTargetData() { TargetAbilitySystem = unit.AbilitySystem } };
        }
      }

      if (eventData != null) {
        _ = SelectedEntity.AbilitySystem.TryActivateAbility(SelectedCommand, eventData);
      }
    }
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
    if (SelectedEntity != null) {
      SelectedEntity.TreeExiting += OnSelectedEntityDied;
      SelectedEntity.CellChanged += OnSelectedEntityCellChanged;
      SelectedEntity.AbilitySystem.GameplayAbilityActivated += OnAbilityActivated;
      SelectedEntity.AbilitySystem.GameplayAbilityEnded += OnAbilityEnded;
    }
  }

  private void OnSelectionStateExited() {
    Character.CancelMoveToPosition();

    if (SelectedEntity != null) {
      SelectedEntity.TreeExiting -= OnSelectedEntityDied;
      SelectedEntity.CellChanged -= OnSelectedEntityCellChanged;
      SelectedEntity.AbilitySystem.GameplayAbilityActivated -= OnAbilityActivated;
      SelectedEntity.AbilitySystem.GameplayAbilityEnded -= OnAbilityEnded;
    }

    SelectedCommand = null;
  }

  private void OnSelectionIdleStateUnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(GameInputs.Focus)) {
      FocusOnSelectedEntity();
    }

    CheckCommandInput(@event);

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
        SelectEntity(entities.FirstOrDefault(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeUnit)), entities.FirstOrDefault()));
      }
    }

    if (@event.IsActionPressed(GameInputs.EndTurn)) {
      TryEndTurn();
    }

    @event.Dispose();
  }

  private void OnSelectionIdleStateEntered() {
    SelectedCommandChanged += UpdateSelectedCommandHighlights;
    if (SelectedEntity != null && SelectedCommand == null) {
      var grantedAbilities = SelectedEntity.AbilitySystem.GetGrantedAbilities();

      var ability = grantedAbilities.FirstOrDefault(s => s.CanActivateAbility(new() { Activator = this }) && s is MoveAbilityResource.Instance);
      ability ??= grantedAbilities.FirstOrDefault(s => s.CanActivateAbility(new() { Activator = this }) && s is AttackAbilityResource.Instance);

      if (ability is HOBAbilityInstance hOBAbility) {
        GetHUD().SelectCommand(hOBAbility);
      }
    }

    UpdateSelectedCommandHighlights();
  }

  private void OnSelectionIdleStateExited() {
    SelectedCommandChanged -= UpdateSelectedCommandHighlights;
  }

  private void OnCommandStateEntered() {
    GetHUD().SetEndTurnButtonDisabled(true);
    SelectedCommand = null;
  }

  private void OnCommandStateExited() {
    GetHUD().SetEndTurnButtonDisabled(false);

  }
  #endregion


  public new IMatchPlayerState GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;

  private void OnSelectedEntityCellChanged() {
    HighlightSystem?.ClearAllHighlights();

    HighlightSystem?.SetHighlight(HighlightType.Selection, SelectedEntity.Cell);

    HighlightSystem?.UpdateHighlights();
  }

  private void OnSelectedEntityDied() {
    SelectEntity(null);
    HighlightSystem?.ClearAllHighlights();
    HighlightSystem?.UpdateHighlights();
  }

  private void OnSelectedEntityChanged() {
    HighlightSystem?.ClearAllHighlights();

    if (SelectedEntity != null) {
      HighlightSystem?.SetHighlight(HighlightType.Selection, SelectedEntity.Cell);
    }
    else {
      SelectedCommand = null;
    }

    HighlightSystem?.UpdateHighlights();
  }

  private void OnSelectedCommandChanged() {

  }

  private void OnCommandSelected() {
    if (SelectedCommand is EntityProductionAbilityResource.Instance) {
      SelectedEntity?.AbilitySystem.TryActivateAbility(SelectedCommand, new() { Activator = this });
    }
  }

  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;

  private void OnAbilityActivated(GameplayAbilityInstance abilityInstance) {
    if (abilityInstance is MoveAbilityResource.Instance or AttackAbilityResource.Instance) {
      StateChart?.SendEvent("command_started");
    }
  }

  private void OnAbilityEnded(GameplayAbilityInstance abilityInstance) {
    StateChart?.SendEvent("command_finished");
  }

  private void UpdateSelectedCommandHighlights() {
    HighlightSystem?.ClearAllHighlights();

    if (SelectedEntity != null) {
      HighlightSystem?.SetHighlight(HighlightType.Selection, SelectedEntity.Cell);
    }

    if (SelectedCommand is MoveAbilityResource.Instance moveAbility) {
      foreach (var cell in moveAbility.GetReachableCells()) {
        HighlightSystem?.SetHighlight(HighlightType.Movement, cell);
      }
    }
    else if (SelectedCommand is AttackAbilityResource.Instance attackAbility) {
      foreach (var cell in attackAbility.GetAttackableEntities().cellsInRange) {
        HighlightSystem?.SetHighlight(HighlightType.Attack, cell);
      }
    }

    HighlightSystem?.UpdateHighlights();
  }

  private void OnEntityAdded(Entity entity) {
    SpawnUIForEntity(entity);
  }

  private void SpawnUIForEntity(Entity entity) {
    var ui3d = ResourceLoader.Load<PackedScene>(_entityUISceneUID).Instantiate<Node3D>();

    var entityUI = ui3d.GetChild(0).GetChild<EntityUI>(0);
    entityUI.Initialize(entity);

    var icon = GetHUD().GetIconFor(entity);
    if (icon != null) {
      entityUI.SetIcon(icon);
    }

    var aabb = new Aabb();
    foreach (var child in entity.Body.GetAllChildren()) {
      if (child is MeshInstance3D mesh) {
        aabb = aabb.Merge(mesh.GetAabb());
      }
    }

    UpdateEntityUIColor(entityUI, entity);

    entity.OwnerControllerChanged += () => {
      UpdateEntityUIColor(entityUI, entity);
    };
    ui3d.Position = aabb.GetCenter() + Vector3.Up * (aabb.Size.Y + 2);

    if (entity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure))) {
      ui3d.Position += Vector3.Up * 2;
    }
    entity.Body.AddChild(ui3d);
  }

  private void UpdateEntityUIColor(EntityUI ui, Entity entity) {
    if (entity.TryGetOwner(out var owner)) {
      if (owner == this) {
        ui.SetTeamColor(Colors.Green);
      }
      else {
        ui.SetTeamColor(Colors.Red);
      }
    }
    else {
      ui.SetTeamColor(Colors.White);
    }
  }
}
