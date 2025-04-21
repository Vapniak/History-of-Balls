namespace HOB;

using System.Linq;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

public partial class HOBHUD : HUD {
  [Signal] public delegate void CommandSelectedEventHandler(HOBAbility.Instance abilityInstance);
  [Signal] public delegate void EndTurnPressedEventHandler();

  [Export] private ShaderMaterial VignetteShaderMaterial { get; set; } = default!;

  [Export] public TimeScaleButtonWidget TimeScaleButtonWidget { get; set; } = default!;
  [Export] private Label? TurnChangedNotificationLabel { get; set; }

  [Export] private EntityPanelWidget EntityPanel { get; set; } = default!;
  [Export] private EntityProductionPanelWidget ProductionPanel { get; set; } = default!;
  [Export] private CommandPanelWidget CommandPanel { get; set; } = default!;

  [Export] private Label ObjectivesLabel { get; set; } = default!;
  [Export] private Button? EndTurnButton { get; set; }
  [Export] private Label? RoundLabel { get; set; }
  [Export] private Label? TurnLabel { get; set; }

  [Export] private FlagWidget FlagWidget { get; set; } = default!;
  [Export] private Label? CountryNameLabel { get; set; }

  [ExportGroup("Resources")]
  [Export] private RichTextLabel? PrimaryResourceNameLabel { get; set; }
  [Export] private Label? PrimaryResourceValueLabel { get; set; }
  [Export] private RichTextLabel? SecondaryResourceNameLabel { get; set; }
  [Export] private Label? SecondaryResourceValueLabel { get; set; }

  private Entity? CurrentEntity { get; set; }


  public override void _Ready() {
    base._Ready();

    EntityPanel.Initialize(GetPlayerController());
    CommandPanel.Initialize(GetPlayerController());
    ProductionPanel.Initialize(GetPlayerController());

    TurnChangedNotificationLabel.Modulate = Colors.Transparent;

    GetPlayerController().GetGameMode().GetMatchEvents().MatchEvent += OnMatchEvent;

    var turnBlocking = GameInstance.GetGameMode<HOBGameMode>().GetTurnManagment();

    turnBlocking.TurnBlocked += () => {
      if (GetPlayerController<IMatchController>().IsCurrentTurn()) {
        EndTurnButton.Disabled = true;
      }
    };
    turnBlocking.TurnUnblocked += () => {
      if (GetPlayerController<IMatchController>().IsCurrentTurn()) {
        EndTurnButton.Disabled = false;
      }
    };

    ObjectivesLabel.Text = GetPlayerController().GetGameMode().MissionData.ObjectivesText;


    CommandPanel.CommandSelected += (command) => EmitSignal(SignalName.CommandSelected, command);

    var playerState = GetPlayerController().GetPlayerState<HOBPlayerState>();


    if (playerState.AbilitySystem.AttributeSystem.TryGetAttributeSet<PlayerAttributeSet>(out var attributeSet)) {
      // UpdatePrimaryResourceValue(playerState.AbilitySystem.GetAttributeCurrentValue(attributeSet.PrimaryResource).GetValueOrDefault().ToString());
      // UpdateSecondaryResourceValue(playerState.AbilitySystem.GetAttributeCurrentValue(attributeSet.SecondaryResource).GetValueOrDefault().ToString());

      playerState.AbilitySystem.AttributeSystem.AttributeValueChanged += (attribute, oldValue, newValue) => {
        if (attribute == attributeSet.PrimaryResource) {
          UpdatePrimaryResourceValue(newValue);
        }

        if (attribute == attributeSet.SecondaryResource) {
          UpdateSecondaryResourceValue(newValue);
        }
      };
    }


    PrimaryResourceNameLabel.Clear();
    PrimaryResourceNameLabel.AddImage(playerState.Country.PrimaryResource.Icon, 16, 16);
    PrimaryResourceNameLabel.AddText($" {playerState.Country.PrimaryResource.Name}: ");

    SecondaryResourceNameLabel.Clear();
    SecondaryResourceNameLabel.AddImage(playerState.Country.SecondaryResource.Icon, 16, 16);
    SecondaryResourceNameLabel.AddText($" {playerState.Country.SecondaryResource.Name}: ");

    FlagWidget.SetFlag(GetPlayerController().GetPlayerState<IMatchPlayerState>().Country?.Flag);
    CountryNameLabel.Text = GetPlayerController().GetPlayerState<IMatchPlayerState>().Country?.Name;
  }

  private void UpdatePrimaryResourceValue(float value) {
    PrimaryResourceValueLabel.Text = value.ToString();
  }

  private void UpdateSecondaryResourceValue(float value) {
    SecondaryResourceValueLabel.Text = value.ToString();
  }

  private void OnMatchEvent(Tag tag) {
    if (tag == TagManager.GetTag(HOBTags.EventTurnStarted)) {
      OnTurnChanged(GetPlayerController().GetGameState().CurrentPlayerIndex);
      OnRoundChanged(GetPlayerController().GetGameState().CurrentRound);
    }
  }

  private void OnTurnChanged(int playerIndex) {
    EndTurnButton.Disabled = !GetPlayerController<IMatchController>().IsCurrentTurn();
    var player = GetPlayerController<IMatchController>().GetGameState().PlayerArray[playerIndex] as HOBPlayerState;

    TurnChangedNotificationLabel.Text = player.PlayerName + " TURN";
    TurnLabel.Text = $"{player.PlayerName} TURN";

    var tween = CreateTween().SetEase(Tween.EaseType.OutIn);
    tween.TweenProperty(TurnChangedNotificationLabel, "modulate", Colors.White, 0.5);
    tween.TweenInterval(1);
    tween.TweenProperty(TurnChangedNotificationLabel, "modulate", Colors.Transparent, 1);

    var vignetteTween = CreateTween();
    vignetteTween.TweenProperty(VignetteShaderMaterial, "shader_parameter/color", player.Country.Color, 1f);
  }

  private void OnRoundChanged(int roundNumber) {
    if (GetPlayerController().GetGameMode() is SurviveGameMode surviveGameMode) {
      RoundLabel.Text = $"ROUND {roundNumber} / {surviveGameMode.RoundsToSurvive}";
    }
  }

  private void OnEndTurnPressed() {
    EmitSignal(SignalName.EndTurnPressed);
  }
  public new HOBPlayerController GetPlayerController() {
    return GetPlayerController<HOBPlayerController>()!;
  }
}
