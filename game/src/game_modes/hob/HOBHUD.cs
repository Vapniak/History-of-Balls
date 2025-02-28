namespace HOB;

using System.Linq;
using System.Threading.Tasks;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class HOBHUD : HUD {
  [Signal] public delegate void EndTurnPressedEventHandler();

  [Export] private Label TurnChangedNotificationLabel { get; set; }
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

  public override void _Ready() {
    base._Ready();

    HideCommandPanel();
    HideStatPanel();
    HideProductionPanel();

    TurnChangedNotificationLabel.Modulate = Colors.Transparent;

    GetPlayerController().GetGameState().TurnChangedEvent += () => {
      OnTurnChanged(GetPlayerController().GetGameState().CurrentPlayerIndex);
    };

    GetPlayerController().GetGameState().RoundStartedEvent += () => {
      OnRoundChanged(GetPlayerController().GetGameState().CurrentRound);
    };

    GetPlayerController().SelectedEntityChanged += () => {
      var selectedEntity = GetPlayerController().SelectedEntity;
      HideProductionPanel();
      if (selectedEntity == null) {
        HideCommandPanel();
        HideStatPanel();
      }
      else {
        if (selectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
          ShowCommandPanel(commandTrait);
        }

        ShowStatPanel(selectedEntity);
      }
    };
  }

  public void OnGameStarted() {
    var playerState = GetPlayerController().GetPlayerState<HOBPlayerState>();

    UpdatePrimaryResourceName(playerState.PrimaryResourceType.Name);
    UpdateSecondaryResourceName(playerState.SecondaryResourceType.Name);

    UpdatePrimaryResourceValue(playerState.PrimaryResourceType.Value.ToString());
    UpdateSecondaryResourceValue(playerState.SecondaryResourceType.Value.ToString());

    playerState.PrimaryResourceType.ValueChanged += () => UpdatePrimaryResourceValue(playerState.PrimaryResourceType.Value.ToString());
    playerState.SecondaryResourceType.ValueChanged += () => UpdateSecondaryResourceValue(playerState.SecondaryResourceType.Value.ToString());
  }

  private void UpdatePrimaryResourceName(string name) {
    PrimaryResourceNameLabel.Text = name + ": ";
  }

  private void UpdatePrimaryResourceValue(string value) {
    PrimaryResourceValueLabel.Text = value;
  }

  private void UpdateSecondaryResourceName(string name) {
    SecondaryResourceNameLabel.Text = name + ": ";
  }

  private void UpdateSecondaryResourceValue(string value) {
    SecondaryResourceValueLabel.Text = value;
  }


  private void OnTurnChanged(int playerIndex) {
    EndTurnButton.Disabled = !GetPlayerController<IMatchController>().IsCurrentTurn();

    TurnChangedNotificationLabel.Text = GetPlayerController<IMatchController>().GetGameState().PlayerArray[playerIndex].PlayerName + " TURN";

    var tween = CreateTween();
    tween.TweenProperty(TurnChangedNotificationLabel, "modulate", Colors.White, 0.5);
    tween.TweenInterval(1);
    tween.TweenProperty(TurnChangedNotificationLabel, "modulate", Colors.Transparent, 1);
  }

  private void OnRoundChanged(int roundNumber) {
    RoundLabel.Text = "ROUND " + roundNumber;
  }

  private void ShowStatPanel(Entity entity) {
    UpdateStatPanel(StatPanel, entity);

    StatPanel.Visible = true;
  }

  private void HideStatPanel() {
    StatPanel.Visible = false;
  }

  private void ShowProductionPanel(ProduceEntityCommand produceEntityCommand) {
    ProductionPanel.ClearEntities();

    ProductionPanel.EntitySelected += onEntitySelected;


    if (produceEntityCommand.GetEntity().TryGetStat<EntityProducerStats>(out var producerStats)) {
      foreach (var entity in producerStats.ProducedEntities) {
        ProductionPanel.AddProducedEntity(entity, GetPlayerController<IMatchController>().GetPlayerState(), produceEntityCommand.CanEntityBeProduced(entity));
      }
    }

    void onEntitySelected(ProducedEntityData data) {
      if (produceEntityCommand.TryProduceEntity(GetPlayerController(), data)) {
        ProductionPanel.Hide();
      }
    }

    void onHidden() {
      ProductionPanel.EntitySelected -= onEntitySelected;
      ProductionPanel.Hidden -= onHidden;
    }
    ProductionPanel.Hidden += onHidden;

    ProductionPanel.Show();
  }

  private void HideProductionPanel() {
    ProductionPanel.Hide();
  }


  private void ShowCommandPanel(CommandTrait commandTrait) {
    CommandPanel.ClearCommands();
    foreach (var command in commandTrait.GetCommands()) {
      if (command.ShowInUI) {
        CommandPanel.AddCommand(command);
      }
    }

    if (CommandPanel.GetCommandCount() == 0) {
      HideCommandPanel();
      return;
    }

    CommandPanel.CommandSelected += onCommandSelected;

    void onCommandSelected(Command command) {
      commandTrait.SelectCommand(command);

      if (command is ProduceEntityCommand produceEntity) {
        ShowProductionPanel(produceEntity);
      }
      else {
        HideProductionPanel();
      }
    }

    void onHidden() {
      CommandPanel.CommandSelected -= onCommandSelected;
      CommandPanel.Hidden -= onHidden;
    }
    CommandPanel.Hidden += onHidden;
    if (!CommandPanel.Visible) {
      CommandPanel.Visible = true;
    }

    CommandPanel.GrabFocus();
  }

  private void HideCommandPanel() => CommandPanel.Visible = false;

  public void SetEndTurnButtonDisabled(bool value) {
    EndTurnButton.Disabled = value;
  }

  public void SelectCommand(Command command) {
    CommandPanel.SelectCommand(command);
  }

  public void SelectCommand(int index) {
    CommandPanel.SelectCommand(index);
  }

  private void UpdateStatPanel(StatPanel panel, Entity entity) {
    switch (entity.GetOwnershipTypeFor(GetPlayerController<IMatchController>())) {
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
      healthStats.CurrentHealthChanged += () => panel.UpdateEntry("Health", healthStats.CurrentHealth.ToString());
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

  public new HOBPlayerController GetPlayerController() {
    return GetPlayerController<HOBPlayerController>();
  }
}
