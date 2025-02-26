namespace HOB;

using System.Linq;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class HOBHUD : HUD {
  [Signal] public delegate void EndTurnPressedEventHandler();

  [Export] private StatPanel StatPanel { get; set; }
  [Export] private EntityProductionPanel ProductionPanel { get; set; }
  [Export] private Button EndTurnButton { get; set; }
  [Export] private CommandPanel CommandPanel { get; set; }
  [Export] private Label RoundLabel { get; set; }

  [ExportGroup("Resources")]
  [Export] private Label PrimaryResourceNameLabel { get; set; }
  [Export] private Label PrimaryResourceValueLabel { get; set; }
  [Export] private Label SecondaryResourceNameLabel { get; set; }
  [Export] private Label SecondaryResourceValueLabel { get; set; }


  private Entity HoveredEntity { get; set; }


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
    EndTurnButton.Disabled = !GetPlayerController<IMatchController>().IsCurrentTurn();
  }

  public void OnRoundChanged(int roundNumber) {
    RoundLabel.Text = "ROUND " + roundNumber;
  }

  public void ShowStatPanel(Entity entity) {
    UpdateStatPanel(StatPanel, entity);

    if (!StatPanel.Visible) {
      StatPanel.Visible = true;
    }
  }

  public void HideStatPanel() {
    StatPanel.Visible = false;
  }

  public void ShowProductionPanel(ProduceEntityCommand produceEntityCommand) {
    ProductionPanel.ClearEntities();

    ProductionPanel.EntitySelected += onEntitySelected;


    if (produceEntityCommand.GetEntity().TryGetStat<EntityProducerStats>(out var producerStats)) {
      foreach (var entity in producerStats.ProducedEntities) {
        ProductionPanel.AddProducedEntity(entity, produceEntityCommand.CanEntityBeProduced(entity));
      }
    }

    void onEntitySelected(int index) {
      produceEntityCommand.TryProduceEntity(index);
      ProductionPanel.Hide();
    }

    void onHidden() {
      ProductionPanel.EntitySelected -= onEntitySelected;
      ProductionPanel.Hidden -= onHidden;
    }
    ProductionPanel.Hidden += onHidden;

    ProductionPanel.Show();
  }

  public void HideProductionPanel() {
    ProductionPanel.Hide();
  }


  public void ShowCommandPanel(CommandTrait commandTrait) {
    CommandPanel.ClearCommands();
    foreach (var command in commandTrait.GetCommands()) {
      if (command.ShowInUI) {
        CommandPanel.AddCommand(command);
      }
    }

    if (CommandPanel.GetCommandCount() == 0) {
      return;
    }

    CommandPanel.CommandSelected += commandTrait.SelectCommand;

    CommandPanel.SelectCommand(commandTrait.GetCommands().FirstOrDefault(c => CommandPanel.CommandCanBeSelected(c) && c is MoveCommand or AttackCommand));


    void onHidden() {
      CommandPanel.CommandSelected -= commandTrait.SelectCommand;
      CommandPanel.Hidden -= onHidden;
    }
    CommandPanel.Hidden += onHidden;
    if (!CommandPanel.Visible) {
      CommandPanel.Visible = true;
    }

    CommandPanel.GrabFocus();
  }

  public void HideCommandPanel() => CommandPanel.Visible = false;

  public void SetEndTurnButtonDisabled(bool value) {
    EndTurnButton.Disabled = value;
  }

  public void SelectCommand(int index) {
    CommandPanel.SelectCommand(index);
  }
  private void UpdateStatPanel(StatPanel panel, Entity entity) {
    switch (entity.GetOwnershipType(GetPlayerController<IMatchController>())) {
      case Entity.OwnershipType.Owned:
        panel.SetNameLabel(entity.GetEntityName());
        break;
      case Entity.OwnershipType.Enemy:
        panel.SetNameLabel("ENEMY " + entity.GetEntityName());
        break;
      case Entity.OwnershipType.NotOwned:
        panel.SetNameLabel("NOT OWNED " + entity.GetEntityName());
        break;
      default:
        break;
    }

    panel.ClearEntries();

    if (entity.TryGetStat<MovementStats>(out var movementStats)) {
      panel.AddEntry("Move Points:", movementStats.MovePoints.ToString());
    }

    if (entity.TryGetStat<HealthStats>(out var healthStats)) {
      panel.AddEntry("Health:", healthStats.CurrentHealth.ToString());
    }

    if (entity.TryGetStat<AttackStats>(out var attackStats)) {
      panel.AddEntry("Damage:", attackStats.Damage.ToString());
      panel.AddEntry("Range:", attackStats.Range.ToString());
    }

    var playerState = GetPlayerController<IMatchController>().GetPlayerState();
    if (entity.TryGetOwner(out var owner)) {
      playerState = owner.GetPlayerState();
    }

    if (entity.TryGetStat<FactoryStats>(out var factoryStats)) {
      var primary = playerState.GetResourceType(factoryStats.InputType);
      var secondary = playerState.GetResourceType(factoryStats.OutputType);
      panel.AddEntry("Input Resource:", primary.Name);
      panel.AddEntry("Input Value:", factoryStats.InputValue.ToString());
      panel.AddEntry("Output Resource:", secondary.Name);
      panel.AddEntry("Output Value:", factoryStats.OutputValue.ToString());
    }

    if (entity.TryGetTrait<FactoryTrait>(out var factoryTrait)) {
      if (factoryTrait.ProcessingRoundsLeft > 0) {
        panel.AddEntry("Processing Turns Left:", factoryTrait.ProcessingRoundsLeft.ToString());
      }
    }

    if (entity.TryGetStat<IncomeStats>(out var incomeStats)) {
      panel.AddEntry("Income Type:", playerState.GetResourceType(incomeStats.IncomeType).Name);
      panel.AddEntry("Income amount:", incomeStats.Value.ToString());
    }
  }

  private void OnEndTurnPressed() {
    EmitSignal(SignalName.EndTurnPressed);
  }

  private void OnProduceEntitySelected(int index) {

  }
}
