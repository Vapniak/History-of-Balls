namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class TestHUD : HUD {
  [Export] private StatPanel StatPanel { get; set; }
  [Export] private CommandPanel CommandPanel { get; set; }
  [Export] private Label RoundLabel { get; set; }
  [Export] private Label PlayerTurnLabel { get; set; }

  public void SetRoundLabel(int round) {
    RoundLabel.Text = "ROUND " + round;
  }

  public void SetPlayerTurnLabel(int playerTurn) {
    PlayerTurnLabel.Text = "PLAYER " + playerTurn + " TURN";
  }

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

  public void ShowCommandPanel(CommandTrait commandTrait) {
    CommandPanel.ClearCommands();
    foreach (var command in commandTrait.GetCommands()) {
      // TODO: callback when command selected
      CommandPanel.AddCommand(command);
    }
    CommandPanel.Show();
  }

  public void HideCommandPanel() => CommandPanel.Hide();
}
