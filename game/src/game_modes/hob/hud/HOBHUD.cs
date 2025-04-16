namespace HOB;

using System.Linq;
using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

public partial class HOBHUD : HUD {
  [Signal] public delegate void CommandSelectedEventHandler(HOBAbilityInstance abilityInstance);
  [Signal] public delegate void EndTurnPressedEventHandler(bool skip);

  [Export] public TimeScaleButtonWidget TimeScaleButtonWidget { get; set; } = default!;
  [Export] private Label? TurnChangedNotificationLabel { get; set; }
  [Export] private EntityPanelWidget EntityPanel { get; set; } = default!;
  [Export] private EntityProductionPanel? ProductionPanel { get; set; }
  [Export] private Button? EndTurnButton { get; set; }
  [Export] private CommandPanelWidget? CommandPanel { get; set; }
  [Export] private Label? RoundLabel { get; set; }
  [Export] private Label? TurnLabel { get; set; }
  [Export] private FlagWidget FlagWidget { get; set; } = default!;
  [Export] private Label? CountryNameLabel { get; set; }
  [Export] private Array<AttributeIcon> AttributeIcons { get; set; } = new();

  [ExportGroup("Resources")]
  [Export] private RichTextLabel? PrimaryResourceNameLabel { get; set; }
  [Export] private Label? PrimaryResourceValueLabel { get; set; }
  [Export] private RichTextLabel? SecondaryResourceNameLabel { get; set; }
  [Export] private Label? SecondaryResourceValueLabel { get; set; }

  private Entity? CurrentEntity { get; set; }


  public override void _Ready() {
    base._Ready();

    HideUIFor(null);

    TurnChangedNotificationLabel.Modulate = Colors.Transparent;

    GetPlayerController().GetGameMode().GetMatchEvents().MatchEvent += OnMatchEvent;

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
      CurrentEntity = selectedEntity;

      if (selectedEntity != null) {
        ShowUIFor(selectedEntity);
      }
    };

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

  private void OnEndTurnPressed(bool speedUp) {
    EmitSignal(SignalName.EndTurnPressed, speedUp);
  }

  private void ShowUIFor(Entity entity) {
    EntityPanel.ClearEntries();

    EntityPanel.BindToEntity(entity, GameInstance.GetGameMode<HOBGameMode>().GetIconFor(entity));

    EntityPanel.Show();

    CommandPanel.ClearCommands();
    ProductionPanel.ClearEntities();


    foreach (var ability in entity.AbilitySystem.GetGrantedAbilities().OrderBy(a => (a.AbilityResource as HOBAbilityResource)?.UIOrder)) {
      if (ability is HOBAbilityInstance hOBAbility && hOBAbility.AbilityResource.ShowInUI) {
        CommandPanel.AddCommand(hOBAbility);
      }

      if (ability is EntityProductionAbilityResource.Instance production) {
        if (entity.TryGetOwner(out var owner)) {
          ProductionPanel.ShowProducedEntities(production, owner.GetPlayerState(), GetPlayerController());
        }
      }
    }

    CommandPanel.Show();
  }
  private void HideUIFor(Entity? entity) {
    EntityPanel.Unbind();
    EntityPanel.Hide();

    ProductionPanel.Hide();
    CommandPanel.Hide();
  }

  public void SelectCommand(HOBAbilityInstance abilityInstance) {
    CommandPanel.SelectCommand(abilityInstance);
  }

  public new HOBPlayerController GetPlayerController() {
    return GetPlayerController<HOBPlayerController>();
  }
}
