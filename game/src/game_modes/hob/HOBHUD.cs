namespace HOB;

using System.Linq;
using System.Threading.Tasks;
using GameplayAbilitySystem;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

public partial class HOBHUD : HUD {
  [Signal] public delegate void CommandSelectedEventHandler(HOBAbilityInstance abilityInstance);
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

    HideUIFor(null);

    TurnChangedNotificationLabel.Modulate = Colors.Transparent;

    GetPlayerController().GetGameMode().GetMatchEvents().TurnChanged += () => {
      OnTurnChanged(GetPlayerController().GetGameState().CurrentPlayerIndex);
    };

    GetPlayerController().GetGameMode().GetMatchEvents().RoundStarted += () => {
      OnRoundChanged(GetPlayerController().GetGameState().CurrentRound);
    };

    GetPlayerController().SelectedEntityChanging += () => {
      var selectedEntity = GetPlayerController().SelectedEntity;

      if (selectedEntity == null) {

      }
      else {
        HideUIFor(selectedEntity);
      }
    };

    GetPlayerController().SelectedEntityChanged += () => {
      var selectedEntity = GetPlayerController().SelectedEntity;

      if (selectedEntity != null) {
        ShowUIFor(selectedEntity);
      }
    };

    CommandPanel.CommandSelected += (command) => EmitSignal(SignalName.CommandSelected, command);
  }

  public void OnGameStarted() {
    var playerState = GetPlayerController().GetPlayerState<HOBPlayerState>();


    // TODO: add resource values
    UpdatePrimaryResourceValue(playerState.PrimaryResourceValue.ToString());
    UpdateSecondaryResourceValue(playerState.SecondaryResourceValue.ToString());


    PrimaryResourceNameLabel.Clear();
    PrimaryResourceNameLabel.AddImage(playerState.Country.PrimaryResource.Icon, 16, 16);
    PrimaryResourceNameLabel.AddText($" {playerState.Country.PrimaryResource.Name}: ");

    SecondaryResourceNameLabel.Clear();
    SecondaryResourceNameLabel.AddImage(playerState.Country.SecondaryResource.Icon, 16, 16);
    SecondaryResourceNameLabel.AddText($" {playerState.Country.SecondaryResource.Name}: ");

    playerState.SecondaryResourceValueChanged += () => UpdatePrimaryResourceValue(playerState.PrimaryResourceValue.ToString());
    playerState.SecondaryResourceValueChanged += () => UpdateSecondaryResourceValue(playerState.SecondaryResourceValue.ToString());

    CountryFlag.Texture = GetPlayerController().GetPlayerState<IMatchPlayerState>().Country?.Flag;
    CountryNameLabel.Text = GetPlayerController().GetPlayerState<IMatchPlayerState>().Country?.Name;
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
  public void SetEndTurnButtonDisabled(bool value) {
    EndTurnButton.Disabled = value;
  }

  private void OnEndTurnPressed() {
    EmitSignal(SignalName.EndTurnPressed);
  }

  private void ShowUIFor(Entity entity) {
    StatPanel.ClearEntries();

    StatPanel.SetNameLabel(entity.EntityName);

    entity.AbilitySystem.AttributeValueChanged += OnAttributeValueChanged;
    foreach (var attribute in entity.AbilitySystem.GetAllAttributes().OrderBy(a => a.AttributeName)) {
      if (attribute != null) {
        StatPanel.AddEntry(attribute.AttributeName, entity.AbilitySystem.GetAttributeCurrentValue(attribute).GetValueOrDefault().ToString());
      }
    }

    StatPanel.Show();

    CommandPanel.ClearCommands();

    foreach (var ability in entity.AbilitySystem.GetGrantedAbilities().OrderBy(a => (a.AbilityResource as HOBAbilityResource)?.UIOrder)) {
      if (ability is HOBAbilityInstance hOBAbility) {
        CommandPanel.AddCommand(hOBAbility);
      }
    }

    foreach (var ability in entity.AbilitySystem.GetGrantedAbilities()) {
      if (ability.CanActivateAbility(null)) {
        if (ability is HOBAbilityInstance hOBAbility) {
          if (ability is MoveAbilityResource.MoveAbilityInstance or AttackAbilityResource.AttackAbilityInstance) {
            CommandPanel.SelectCommand(hOBAbility);
            break;
          }
        }
      }
    }

    CommandPanel.Show();
  }
  private void HideUIFor(Entity? entity) {
    if (entity != null) {
      entity.AbilitySystem.AttributeValueChanged -= OnAttributeValueChanged;
    }
    StatPanel.Hide();
    CommandPanel.Hide();
  }

  private void OnAttributeValueChanged(GameplayAttribute attribute, float oldValue, float newValue) {
    StatPanel.UpdateEntry(attribute.AttributeName, newValue.ToString());
  }

  public new HOBPlayerController GetPlayerController() {
    return GetPlayerController<HOBPlayerController>();
  }
}
