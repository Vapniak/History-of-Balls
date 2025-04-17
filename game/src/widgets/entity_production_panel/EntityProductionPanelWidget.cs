namespace HOB;

using Godot;
using HOB.GameEntity;
using WidgetSystem;

public partial class EntityProductionPanelWidget : Widget {
  [Export] private Control? EntitiesList { get; set; }
  private EntityProductionAbilityResource.Instance? _ability;
  private HOBPlayerController _playerController = default!;

  public override void _Ready() {
    Hide();
  }

  public void Initialize(HOBPlayerController playerController) {
    _playerController = playerController;

    playerController.SelectedEntityChanged += () => {
      var entity = playerController.SelectedEntity;

      if (entity != null) {
        ShowProducedEntities(entity);
      }
      else {
        ClearEntities();
      }
    };
  }

  public void ClearEntities() {
    Hide();
    if (EntitiesList == null) {
      return;
    }

    _ability = null;
    foreach (var child in EntitiesList.GetChildren()) {
      child.Free();
    }
  }

  public void ShowProducedEntities(Entity entity) {
    ClearEntities();

    var ability = entity.AbilitySystem.GetGrantedAbility<EntityProductionAbilityResource.Instance>();

    if (ability == null) {
      return;
    }

    if (!entity.TryGetOwner(out var owner)) {
      return;
    }

    var playerState = owner.GetPlayerState();

    foreach (var config in playerState.ProducedEntities) {
      var widget = ProductionEntryWidget.CreateWidget();
      widget.BindTo(_playerController, ability, config);
      EntitiesList?.AddChild(widget);
    }

    if (GetEntriesCount() > 0) {
      Show();
    }
    else {
      Hide();
    }
  }

  public int GetEntriesCount() => EntitiesList?.GetChildCount() ?? 0;
}
