namespace HOB;

using Godot;
using HOB.GameEntity;

public partial class EntityProductionPanelWidget : HOBWidget {
  [Export] private Control EntitiesList { get; set; } = default!;
  [Export] private Label CurrentProductionLabel { get; set; } = default!;
  [Export] private Label RoundsLeftLabel { get; set; } = default!;

  private EntityProductionAbility.Instance? BoundAbility { get; set; }
  private HOBPlayerController _playerController = default!;

  public override void _Ready() {
    Hide();
  }

  public void Initialize(HOBPlayerController playerController) {
    _playerController = playerController;

    playerController.SelectedEntityChanged += () => {
      var entity = playerController.SelectedEntity;

      BindTo(entity);
    };
  }

  public void ClearEntities() {
    Hide();
    if (EntitiesList == null) {
      return;
    }

    foreach (var child in EntitiesList.GetChildren()) {
      child.Free();
    }
  }

  private void BindTo(Entity? entity) {
    ClearEntities();

    if (BoundAbility != null) {
      BoundAbility.RoundsLeftChanged -= updateLabels;
    }

    if (entity == null) {
      BoundAbility = null;
      return;
    }

    BoundAbility = entity.AbilitySystem.GetGrantedAbility<EntityProductionAbility.Instance>();

    if (BoundAbility == null) {
      return;
    }

    void updateLabels(int roundsLeft) {
      if (BoundAbility.CurrentProduction != null && roundsLeft > 0) {
        RoundsLeftLabel.Text = "Rounds Left: " + roundsLeft;
        CurrentProductionLabel.Text = "Producing: " + BoundAbility.CurrentProduction.Entity.EntityName;

        RoundsLeftLabel.Show();
        CurrentProductionLabel.Show();
      }
      else {
        RoundsLeftLabel.Hide();
        CurrentProductionLabel.Hide();
      }
    }

    updateLabels(BoundAbility.RoundsLeft);
    BoundAbility.RoundsLeftChanged += updateLabels;


    if (!entity.TryGetOwner(out var owner)) {
      return;
    }

    var playerState = owner.GetPlayerState();

    foreach (var config in playerState.ProducedEntities) {
      var widget = ProductionEntryWidget.CreateWidget();
      widget.BindTo(_playerController, BoundAbility, config);
      EntitiesList.AddChild(widget);
    }

    if (EntitiesList.GetChildCount() > 0) {
      Show();
    }
    else {
      Hide();
    }
  }
}
