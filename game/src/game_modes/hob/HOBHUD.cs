namespace HOB;

using System.Linq;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class HOBHUD : HUD {
  [Signal] public delegate void EndTurnEventHandler();

  [Export] private StatPanel StatPanel { get; set; }
  [Export] private StatPanel HoverStatPanel { get; set; }
  [Export] private Button EndTurnButton { get; set; }
  [Export] private CommandPanel CommandPanel { get; set; }
  [Export] private Label RoundLabel { get; set; }
  [Export] private Label PlayerTurnLabel { get; set; }

  [ExportGroup("Resources")]
  [Export] private Label PrimaryResourceNameLabel { get; set; }
  [Export] private Label PrimaryResourceValueLabel { get; set; }
  [Export] private Label SecondaryResourceNameLabel { get; set; }
  [Export] private Label SecondaryResourceValueLabel { get; set; }


  private Entity HoveredEntity { get; set; }

  public override void _Process(double delta) {
    if (HoverStatPanel.Visible && IsInstanceValid(HoveredEntity)) {
      // TODO: check if not obsuring other ui and is in view
      HoverStatPanel.Position = GetViewport().GetCamera3D().UnprojectPosition(HoveredEntity.GetPosition());
    }
  }

  public void UpdatePrimaryResourceName(string name) {
    PrimaryResourceNameLabel.Text = name + ": ";
  }

  public void UpdatePrimaryResourceValue(string value) {
    PrimaryResourceValueLabel.Text = value;
  }

  public void UpdateSecondaryResourceName(string name) {
    SecondaryResourceNameLabel.Text = name + ": ";
  }

  public void UpdateSecondaryResourceValue(string value) {
    SecondaryResourceValueLabel.Text = value;
  }


  public void OnTurnChanged(int playerIndex) {
    PlayerTurnLabel.Text = "PLAYER " + playerIndex + " TURN";

    EndTurnButton.Disabled = !GetPlayerController<IMatchController>().IsCurrentTurn();
  }

  public void OnRoundChanged(int roundNumber) {
    RoundLabel.Text = "ROUND " + roundNumber;
  }

  public void ShowStatPanel(Entity entity) {
    UpdateStatPanel(StatPanel, entity);

    StatPanel.Show();
  }

  public void HideStatPanel() {
    StatPanel.Hide();
  }

  public void ShowHoverStatPanel(Entity entity) {

    HoveredEntity = entity;

    UpdateStatPanel(HoverStatPanel, entity);

    HoverStatPanel.Position = GetViewport().GetCamera3D().UnprojectPosition(entity.GetPosition());
    HoverStatPanel.Show();
  }

  public void HideHoverStatPanel() {
    HoveredEntity = null;
    HoverStatPanel.Hide();
  }


  public void ShowCommandPanel(CommandTrait commandTrait) {
    CommandPanel.ClearCommands();
    CommandPanel.GrabFocus();
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

  public void SetEndTurnButtonDisabled(bool value) {
    EndTurnButton.Disabled = value;
  }

  public void SelectCommand(int index) {
    CommandPanel.SelectCommand(index);
  }
  private void UpdateStatPanel(StatPanel panel, Entity entity) {
    var isOwned = entity.IsOwnedBy(GetPlayerController<IMatchController>());

    if (isOwned) {
      panel.SetEntityName(entity.GetEntityName());
    }
    else {
      if (entity.OwnerController != null) {
        panel.SetEntityName("ENEMY " + entity.GetEntityName());
      }
      else {
        panel.SetEntityName("NOT OWNED " + entity.GetEntityName());
      }
    }

    panel.ClearEntries();

    if (entity.TryGetStat<MovementStats>(out var movementStats)) {
      panel.AddEntry("Move Points:", movementStats.MovePoints.ToString());
    }

    if (entity.TryGetStat<HealthStats>(out var healthStats)) {
      panel.AddEntry("Health:", healthStats.CurrentHealth.ToString());
    }

    if (entity.TryGetStat<AttackStats>(out var attackStats)) {
      panel.AddEntry("Damage: ", attackStats.Damage.ToString());
      panel.AddEntry("Range: ", attackStats.Range.ToString());
    }
  }

  private void OnEndTurnPressed() {
    EmitSignal(SignalName.EndTurn);
  }

}
