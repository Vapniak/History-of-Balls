namespace HOB;

using System;
using GameplayFramework;
using GameplayTags;
using Godot;
using HOB.GameEntity;

public partial class HOBHUD : HUD {
  [Signal] public delegate void CommandSelectedEventHandler(HOBAbility.Instance abilityInstance);
  [Signal] public delegate void EndTurnPressedEventHandler();

  [Export] public Button PauseButton { get; private set; } = default!;

  [Export] private ShaderMaterial VignetteShaderMaterial { get; set; } = default!;
  [Export] private Material PanelBackgroundMaterial { get; set; } = default!;

  [Export] public TimeScaleButtonWidget TimeScaleButtonWidget { get; set; } = default!;
  [Export] private Label? TurnChangedNotificationLabel { get; set; }
  [Export] private TextureRect CountryBallRender { get; set; } = default!;

  [Export] private EntityPanelWidget EntityPanel { get; set; } = default!;
  [Export] private EntityProductionPanelWidget ProductionPanel { get; set; } = default!;
  [Export] private CommandPanelWidget CommandPanel { get; set; } = default!;

  [Export] public Control ObjectivesListParent { get; set; } = default!;
  [Export] private Button? EndTurnButton { get; set; }
  [Export] private Label? RoundLabel { get; set; }
  [Export] private Label? TurnLabel { get; set; }

  [Export] private FlagWidget FlagWidget { get; set; } = default!;
  [Export] private Label? CountryNameLabel { get; set; }

  [ExportGroup("Resources")]
  [Export] private ResourceWidget PrimaryResourceWidget { get; set; } = default!;
  [Export] private ResourceWidget SecondaryResourceWidget { get; set; } = default!;


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

    //ObjectivesLabel.Text = GetPlayerController().GetGameMode().MissionData.ObjectivesText;
    var objectivesText = GetPlayerController().GetGameMode().MissionData.ObjectivesText;
    var objectives = objectivesText.Split("\n");
    foreach (var objective in objectives) {
      var widget = ObjectiveWidget.CreateWidget();
      widget.Label.AppendText(objective);

      ObjectivesListParent.AddChild(widget);
    }

    CommandPanel.CommandSelected += (command) => EmitSignal(SignalName.CommandSelected, command);

    var playerState = GetPlayerController().GetPlayerState<HOBPlayerState>();
    GetChild<Control>(0).Theme = playerState.Theme;

    PanelBackgroundMaterial.Set("shader_parameter/tint", playerState.Country.Color);
    CountryBallRender.Texture = playerState.Country.CountryBallRender;

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


    PrimaryResourceWidget.Icon.Texture = playerState.Country.PrimaryResource.Icon;
    //PrimaryResourceNameLabel.AddText($" {playerState.Country.PrimaryResource.Name}: ");
    PrimaryResourceWidget.TooltipText = playerState.Country.PrimaryResource.Name + "(Primary Resource)";

    SecondaryResourceWidget.Icon.Texture = playerState.Country.SecondaryResource.Icon;
    SecondaryResourceWidget.TooltipText = playerState.Country.SecondaryResource.Name + "(Secondary Resource)";
    //SecondaryResourceNameLabel.AddText($" {playerState.Country.SecondaryResource.Name}: ");

    FlagWidget.SetFlag(GetPlayerController().GetPlayerState<IMatchPlayerState>().Country?.Flag);
    CountryNameLabel.Text = GetPlayerController().GetPlayerState<IMatchPlayerState>().Country?.Name;
  }

  private void UpdatePrimaryResourceValue(float value) {
    PrimaryResourceWidget.ValueLabel.Text = value.ToString();
  }

  private void UpdateSecondaryResourceValue(float value) {
    SecondaryResourceWidget.ValueLabel.Text = value.ToString();
  }

  private void OnMatchEvent(Tag tag) {
    if (tag == TagManager.GetTag(HOBTags.EventTurnStarted)) {
      OnTurnChanged(GetPlayerController().GetGameState<IMatchGameState>().CurrentPlayerIndex);
      OnRoundChanged(GetPlayerController().GetGameState<IMatchGameState>().CurrentRound);
    }
  }

  private void OnTurnChanged(int playerIndex) {
    EndTurnButton.Disabled = !GetPlayerController<IMatchController>().IsCurrentTurn();
    var player = GetPlayerController<IMatchController>().GetGameState<IMatchGameState>().PlayerArray[playerIndex] as HOBPlayerState;

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
      RoundLabel.Text = $"{roundNumber} / {surviveGameMode.RoundsToSurvive}";
    }
    else {
      RoundLabel.Text = roundNumber.ToString();
    }
  }

  private void OnEndTurnPressed() {
    EmitSignal(SignalName.EndTurnPressed);
  }

  public new HOBPlayerController GetPlayerController() {
    return GetPlayerController<HOBPlayerController>()!;
  }
}
