namespace HOB;

using System;
using System.Linq;
using System.Threading.Tasks;
using AudioManager;
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
  [Export] private Material? TransparentMaterial { get; set; }

  [Notify] public Entity? SelectedEntity { get => _selectedEntity.Get(); private set => _selectedEntity.Set(value); }
  [Notify] public GameCell? HoveredCell { get => _hoveredCell.Get(); private set => _hoveredCell.Set(value); }

  [Notify] public HOBAbility.Instance? SelectedCommand { get => _selectedCommand.Get(); private set => _selectedCommand.Set(value); }

  private StateChart? StateChart { get; set; }

  private GameBoard GameBoard => GetGameState().GameBoard;
  private PlayerCharacter Character => GetCharacter<PlayerCharacter>();

  private bool _gameStarted;
  private bool _isPanning;
  private Vector2 _lastMousePosition;

  private HighlightSystem? Highlight { get; set; }
  private IEntityManagment EntityManagment => GetGameMode().GetEntityManagment();

  public override void _Ready() {
    base._Ready();

    Highlight = new(this, HighlightColors ?? new(), GameBoard);
    AddChild(Highlight);

    GetHUD().EndTurnPressed += TryEndTurn;
    GetHUD().PauseButton.Pressed += () => {
      if (!GetTree().Paused) {
        GetGameMode().Pause();
      }
    };

    SelectedEntityChanged += OnSelectedEntityChanged;
    SelectedCommandChanged += OnSelectedCommandChanged;
    HoveredCellChanged += OnHoveredCellChanged;

    Character.CenterPositionOn(GameBoard.GetAabb());

    if (StateChartNode != null) {
      StateChart = StateChart.Of(StateChartNode);
    }

    GetHUD().CommandSelected += (command) => {
      SelectedCommand = command;
    };

    GetHUD().TimeScaleButtonWidget.Button.Pressed += ToggleTimeScale;

    if (GetPlayerState().AbilitySystem.AttributeSystem.TryGetAttributeSet<PlayerAttributeSet>(out var set)) {
      GameAssetsRegistry.Instance.AttributeIcons.Add(new(set.PrimaryResource, GetPlayerState().Country.PrimaryResource.Icon));
      GameAssetsRegistry.Instance.AttributeIcons.Add(new(set.SecondaryResource, GetPlayerState().Country.SecondaryResource.Icon));
    }

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

    @event.Dispose();
  }

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;
  public override HOBHUD GetHUD() => base.GetHUD() as HOBHUD;

  public override void _Process(double delta) {
    Character.Update(delta / Engine.TimeScale);

    Character.ClampPosition(GameBoard.GetAabb());
  }

  public void TryEndTurn() {
    GetGameMode().GetTurnManagment().TryEndTurn(this);
  }
  public void OwnTurnStarted() {

  }
  public void OwnTurnEnded() {

  }

  public void OnGameStarted() {
    SelectEntity(EntityManagment.GetOwnedEntites(this).FirstOrDefault());
    _ = FocusOnSelectedEntity();

    _gameStarted = true;
  }

  private void ToggleTimeScale() {
    Engine.TimeScale += 1;
    Engine.TimeScale = Mathf.Wrap(Engine.TimeScale, 1, 4);
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

    if (HoveredCell == null || _isPanning || (viewport.GuiGetHoveredControl() != null && viewport.GuiGetHoveredControl().GetParent() is Control control && control.Visible)) {
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
      if (SelectedCommand is MoveAbility.Instance moveAbility) {
        eventData = new() {
          Activator = this,
          TargetData = new MoveTargetData() { Cell = HoveredCell }
        };
      }
      else if (SelectedCommand is AttackAbility.Instance attackAbility) {
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

  private async Task FocusOnSelectedEntity() {
    if (SelectedEntity != null) {
      await Character.MoveToPosition(SelectedEntity.GetPosition(), 1, Tween.TransitionType.Cubic);
      Character.SetZoom(1);
    }
  }
  private void HandleMovement(float delta) {
    delta /= (float)Engine.TimeScale;
    if (!_gameStarted) {
      return;
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

    @event.Dispose();
  }

  private void OnSelectionStateEntered() {
    if (SelectedEntity != null) {
      SelectedEntity.TreeExiting += OnSelectedEntityDied;
      SelectedEntity.AbilitySystem.GameplayAbilityActivated += OnAbilityActivated;
      SelectedEntity.AbilitySystem.GameplayAbilityEnded += OnAbilityEnded;
    }
  }

  private void OnSelectionStateExited() {
    Character.CancelMoveToPosition();

    if (SelectedEntity != null) {
      SelectedEntity.TreeExiting -= OnSelectedEntityDied;
      SelectedEntity.AbilitySystem.GameplayAbilityActivated -= OnAbilityActivated;
      SelectedEntity.AbilitySystem.GameplayAbilityEnded -= OnAbilityEnded;
    }

    SelectedCommand = null;
  }

  private void OnSelectionIdleStateUnhandledInput(InputEvent @event) {
    if (@event.IsActionPressed(GameInputs.Focus)) {
      _ = FocusOnSelectedEntity();
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
    if (SelectedEntity != null && SelectedCommand == null) {
      var grantedAbilities = SelectedEntity.AbilitySystem.GetGrantedAbilities();

      bool checkAbility(GameplayAbility.Instance ability) {
        if (ability is HOBAbility.EntityInstance entityAbility) {
          if (entityAbility.OwnerEntity.TryGetOwner(out var owner) && owner == this) {
            return entityAbility.CheckCooldown();
          }
        }

        return true;
      }
      var ability = grantedAbilities.FirstOrDefault(s => checkAbility(s) && s is MoveAbility.Instance);
      ability ??= grantedAbilities.FirstOrDefault(s => checkAbility(s) && s is AttackAbility.Instance);

      if (ability != null) {
        SelectedCommand = (HOBAbility.Instance)ability;
      }
    }
  }

  private void OnSelectionIdleStateExited() {

  }

  private void OnCommandStateEntered() {
    SelectedCommand = null;
  }

  private void OnCommandStateExited() {

  }
  #endregion


  public new IMatchPlayerState GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;

  private void OnSelectedEntityDied() {
    SelectEntity(null);
  }

  private void OnSelectedEntityChanged() {
    if (SelectedEntity == null) {
      SelectedCommand = null;
    }
    else {
      SoundManager.Instance.Play("ui", "click");
    }
  }


  private void OnHoveredCellChanged() {
    if (HoveredCell == null) {
      return;
    }

    if (EntityManagment.GetEntitiesOnCell(HoveredCell).Length > 0) {
      SoundManager.Instance.Play("ui", "hover");
    }
  }
  private void OnSelectedCommandChanged() {

  }

  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;

  private void OnAbilityActivated(GameplayAbility.Instance abilityInstance) {
    if (abilityInstance is MoveAbility.Instance or AttackAbility.Instance) {
      StateChart?.SendEvent("command_started");
    }
  }

  private void OnAbilityEnded(GameplayAbility.Instance abilityInstance) {
    StateChart?.SendEvent("command_finished");
  }
}
