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
  [Export] private Label TurnLabel { get; set; }
  [Export] private TextureRect CountryFlag { get; set; }
  [Export] private Label CountryNameLabel { get; set; }

  [ExportGroup("Resources")]
  [Export] private RichTextLabel PrimaryResourceNameLabel { get; set; }
  [Export] private Label PrimaryResourceValueLabel { get; set; }
  [Export] private RichTextLabel SecondaryResourceNameLabel { get; set; }
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

    GetPlayerController().SelectedEntityChanging += () => {
      var selectedEntity = GetPlayerController().SelectedEntity;

      if (selectedEntity != null && selectedEntity.TryGetTrait<CommandTrait>(out var commandTrait)) {
        commandTrait.CommandStarted -= onCommandStarted;
        commandTrait.CommandFinished -= onCommandFinished;
      }
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

          commandTrait.CommandStarted += onCommandStarted;
          commandTrait.CommandFinished += onCommandFinished;
        }

        ShowStatPanel(selectedEntity);
      }
    };

    GetPlayerController().SelectedCommandChanged += () => {
      var command = GetPlayerController().SelectedCommand;

      if (command != null && command.GetEntity().TryGetOwner(out var owner) && owner == GetPlayerController() && command is ProduceEntityCommand produceEntity) {
        ShowProductionPanel(produceEntity);
      }
      else {
        HideProductionPanel();
      }
    };

    CommandPanel.CommandSelected += (command) => {
      GetPlayerController().OnHUDCommandSelected(command);
    };

    void onCommandStarted(Command command) {
      UpdateStatPanel(StatPanel, command.GetEntity());
    }

    void onCommandFinished(Command command) {
      UpdateStatPanel(StatPanel, command.GetEntity());
    }
  }

  public void OnGameStarted() {
    var playerState = GetPlayerController().GetPlayerState<HOBPlayerState>();


    UpdatePrimaryResourceValue(playerState.PrimaryResourceType.Value.ToString());
    UpdateSecondaryResourceValue(playerState.SecondaryResourceType.Value.ToString());


    PrimaryResourceNameLabel.Clear();
    PrimaryResourceNameLabel.AddImage(playerState.PrimaryResourceType.Icon, 16, 16);
    PrimaryResourceNameLabel.AddText($" {playerState.PrimaryResourceType.Name}: ");

    SecondaryResourceNameLabel.Clear();
    SecondaryResourceNameLabel.AddImage(playerState.SecondaryResourceType.Icon, 16, 16);
    SecondaryResourceNameLabel.AddText($" {playerState.SecondaryResourceType.Name}: ");

    playerState.PrimaryResourceType.ValueChanged += () => UpdatePrimaryResourceValue(playerState.PrimaryResourceType.Value.ToString());
    playerState.SecondaryResourceType.ValueChanged += () => UpdateSecondaryResourceValue(playerState.SecondaryResourceType.Value.ToString());

    CountryFlag.Texture = GetPlayerController().Country.Flag;
    CountryNameLabel.Text = GetPlayerController().Country.Name;
  }

  private void UpdatePrimaryResourceValue(string value) {
    PrimaryResourceValueLabel.Text = value;
  }

  private void UpdateSecondaryResourceValue(string value) {
    SecondaryResourceValueLabel.Text = value;
  }


  private void OnTurnChanged(int playerIndex) {
    EndTurnButton.Disabled = !GetPlayerController<IMatchController>().IsCurrentTurn();

    var player = GetPlayerController<IMatchController>().GetGameState().PlayerArray[playerIndex];

    TurnChangedNotificationLabel.Text = player.PlayerName + " TURN";
    TurnLabel.Text = $"{player.PlayerName} TURN";

    var tween = CreateTween();
    tween.TweenProperty(TurnChangedNotificationLabel, "modulate", Colors.White, 0.5);
    tween.TweenInterval(1);
    tween.TweenProperty(TurnChangedNotificationLabel, "modulate", Colors.Transparent, 1);
  }

  private void OnRoundChanged(int roundNumber) {
    RoundLabel.Text = $"ROUND {roundNumber}";
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
      if (GetPlayerController<IMatchController>().IsCurrentTurn()) {
        if (produceEntityCommand.TryStartProduceEntity(data)) {
          ShowProductionPanel(produceEntityCommand);
          UpdateStatPanel(StatPanel, produceEntityCommand.GetEntity());
        }
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
      if (command.Data.ShowInUI) {
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
  }

  private void HideCommandPanel() => CommandPanel.Visible = false;

  public void SetEndTurnButtonDisabled(bool value) {
    EndTurnButton.Disabled = value;
  }

  private void SelectCommand(Command command) {
    CommandPanel.SelectCommand(command);
  }

  private void UpdateStatPanel(StatPanel panel, Entity entity) {
    panel.ClearEntries();

    panel.SetNameLabel(entity.GetEntityName());
    if (entity.TryGetStat<MovementStats>(out var movementStats)) {
      panel.AddEntry("Move Points:", movementStats.MovePoints.ToString());
    }

    if (entity.TryGetStat<HealthStats>(out var healthStats)) {
      panel.AddEntry("Health:", healthStats.CurrentHealth.ToString());
      // FIXME: this is unsubscribed
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
          factoryTrait.ProcessingRoundsLeftChanged += () => panel.UpdateEntry("Processing Turns Left:", factoryTrait.ProcessingRoundsLeft.ToString());
        }
      }
    }

    if (entity.TryGetStat<IncomeStats>(out var incomeStats)) {
      panel.AddEntry("Income Type:", playerState.GetResourceType(incomeStats.IncomeType).Name);
      panel.AddEntry("Income Amount:", incomeStats.Value.ToString());
    }

    if (owner == GetPlayerController()) {
      if (entity.TryGetTrait<EntityProducerTrait>(out var producerTrait)) {
        if (producerTrait.CurrentProducedEntity != null) {
          panel.AddEntry("Producing Entity:", producerTrait.CurrentProducedEntity.Entity.EntityName);
          panel.AddEntry("Rounds Left:", producerTrait.ProductionRoundsLeft.ToString());

          producerTrait.ProductionRoundsLeftChanged += () => panel.UpdateEntry("Rounds Left:", producerTrait.ProductionRoundsLeft.ToString());
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
