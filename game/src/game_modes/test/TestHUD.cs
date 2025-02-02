namespace HOB;

using System.Linq;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class TestHUD : HUD {
  [Signal] public delegate void EndTurnEventHandler();

  [Export] private StatPanel StatPanel { get; set; }
  [Export] private Button EndTurnButton { get; set; }
  [Export] private CommandPanel CommandPanel { get; set; }
  [Export] private Label RoundLabel { get; set; }
  [Export] private Label PlayerTurnLabel { get; set; }

  public void OnTurnChanged(int playerIndex) {
    PlayerTurnLabel.Text = "PLAYER " + playerIndex + " TURN";

    EndTurnButton.Disabled = !GetPlayerController<IMatchController>().IsCurrentTurn();
  }

  public void OnRoundChanged(int roundNumber) {
    RoundLabel.Text = "ROUND " + roundNumber;
  }

  public void ShowStatPanel(Entity entity) {
    StatPanel.Visible = true;

    UpdateStatPanel(entity);
  }

  public void HideStatPanel() {
    StatPanel.Visible = false;
  }


  public void ShowCommandPanel(CommandTrait commandTrait) {
    // TODO: activate move command on show if exists

    CommandPanel.ClearCommands();
    foreach (var command in commandTrait.GetCommands()) {
      CommandPanel.AddCommand(command);
    }

    CommandPanel.CommandSelected += commandTrait.SelectCommand;
    CommandPanel.SelectCommand(commandTrait.GetCommands().FirstOrDefault(c => c.IsAvailable() && c is MoveCommand or AttackCommand));


    void onHidden() {
      CommandPanel.CommandSelected -= commandTrait.SelectCommand;
      CommandPanel.Hidden -= onHidden;
    }
    CommandPanel.Hidden += onHidden;
    CommandPanel.Show();
  }

  public void HideCommandPanel() => CommandPanel.Hide();
  private void UpdateStatPanel(Entity entity) {
    var isOwned = entity.IsOwnedBy(GetPlayerController<IMatchController>());

    if (isOwned) {
      StatPanel.SetEntityName(entity.EntityName);
    }
    else {
      StatPanel.SetEntityName("ENEMY " + entity.EntityName);
    }

    StatPanel.ClearEntries();

    if (isOwned && entity.TryGetTrait<MoveTrait>(out var moveTrait)) {
      StatPanel.AddEntry("Move Points:", moveTrait.MovePoints.ToString());
    }

    if (entity.TryGetTrait<HealthTrait>(out var healthTrait)) {
      StatPanel.AddEntry("Health:", healthTrait.CurrentHealth.ToString());
    }
  }

  private void OnEndTurnPressed() {
    EmitSignal(SignalName.EndTurn);
  }
}
