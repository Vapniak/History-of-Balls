namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;
using System;

public partial class TestHUD : HUD {
  [Export] private StatPanel StatPanel { get; set; }
  [Export] private CommandPanel CommandPanel { get; set; }


  public void ShowStatPanel(Entity entity) {
    StatPanel.Visible = true;

    UpdateStatPanel(entity);
  }

  public void HideStatPanel() {
    StatPanel.Visible = false;
  }

  private void UpdateStatPanel(Entity entity) {
    StatPanel.SetNameLabel(entity.EntityName);
    if (entity.TryGetTrait<MoveTrait>(out var moveTrait)) {
      StatPanel.SetMovePointsLabel((int)moveTrait.Data.MovePoints);
    }
  }

  public void ShowCommandPanel(Entity entity) {
    CommandPanel.Show();
  }

  public void HideCommandPanel() => CommandPanel.Hide();
}
