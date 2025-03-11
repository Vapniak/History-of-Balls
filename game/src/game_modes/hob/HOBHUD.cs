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
  [Export] private TextureRect CountryFlag { get; set; }
  [Export] private Label CountryNameLabel { get; set; }

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

    GetPlayerController().GetGameMode().GetMatchEvents().TurnChanged += () => {
      OnTurnChanged(GetPlayerController().GetGameState().CurrentPlayerIndex);
    };

    GetPlayerController().GetGameMode().GetMatchEvents().RoundStarted += () => {
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

    GetPlayerController().SelectedCommandChanged += () => {
      var command = GetPlayerController().SelectedCommand;

      if (command is ProduceEntityCommand produceEntity) {
        ShowProductionPanel(produceEntity);
      }
      else {
        HideProductionPanel();
      }
    };


    CommandPanel.CommandSelected += (command) => {
      GetPlayerController().OnHUDCommandSelected(command);
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

    CountryFlag.Texture = GetPlayerController().Country.Flag;
    CountryNameLabel.Text = GetPlayerController().Country.Name;
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
        ProductionPanel.AddProducedEntity(entity, GetPlayerController<IMatchController>().GetPlayerState(), produceEntityCommand.CanEntityBeProduced(entity) && produceEntityCommand.GetEntity().TryGetOwner(out var owner) && owner == GetPlayerController() && produceEntityCommand.ProducerTrait.ProductionRoundsLeft == 0);
      }
    }

    void onEntitySelected(ProducedEntityData data) {
      if (produceEntityCommand.TryStartProduceEntity(GetPlayerController(), data)) {
        ShowProductionPanel(produceEntityCommand);
        UpdateStatPanel(StatPanel, produceEntityCommand.GetEntity());
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

    if (!CommandPanel.Visible) {
      CommandPanel.Visible = true;
    }

    CommandPanel.GrabFocus();
  }

  private void HideCommandPanel() => CommandPanel.Visible = false;

  public void SetEndTurnButtonDisabled(bool value) {
    EndTurnButton.Disabled = value;
  }

  private void SelectCommand(Command command) {
    CommandPanel.SelectCommand(command);
  }

  private void UpdateStatPanel(StatPanel panel, Entity entity) {
    panel.SetNameLabel(entity.GetEntityName());


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
      var primary = playerState.GetResourceType(factoryStats.ProcessedResource);
      var secondary = playerState.GetResourceType(factoryStats.ProducedResource);
      panel.AddEntry("Processed Resource:", primary.Name);
      panel.AddEntry("Processed Value:", factoryStats.ProcessedValue.ToString());
      panel.AddEntry("Produced Resource:", secondary.Name);
      panel.AddEntry("Produced Value:", factoryStats.ProducedValue.ToString());
    }

    if (owner == GetPlayerController()) {
      if (entity.TryGetTrait<FactoryTrait>(out var factoryTrait)) {
        if (factoryTrait.ProcessingRoundsLeft > 0) {
          panel.AddEntry("Processing Turns Left:", factoryTrait.ProcessingRoundsLeft.ToString());
        }
      }
    }

    if (entity.TryGetStat<IncomeStats>(out var incomeStats)) {
      panel.AddEntry("Income Type:", playerState.GetResourceType(incomeStats.IncomeType).Name);
      panel.AddEntry("Income Amount:", incomeStats.Value.ToString());
    }

    if (owner == GetPlayerController()) {
      if (entity.TryGetTrait<EntityProducerTrait>(out var producerTrait)) {
        if (producerTrait.ProductionRoundsLeft > 0) {
          panel.AddEntry("Producing Entity:", producerTrait.CurrentProducedEntity.Entity.EntityName);
          panel.AddEntry("Rounds Left:", producerTrait.ProductionRoundsLeft.ToString());
        }
      }
    }
  }

  private void OnEndTurnPressed() {
    EmitSignal(SignalName.EndTurnPressed);
  }

  public new HOBPlayerController GetPlayerController() {
    return GetPlayerController<HOBPlayerController>();
  }
}
