namespace HOB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GameplayFramework;
using Godot;
using GodotStateCharts;
using HOB.GameEntity;

[GlobalClass]
public partial class HOBGameMode : GameMode {
  [Export] private MatchEndMenu MatchEndMenu { get; set; }
  [Export] private PauseMenu PauseMenu { get; set; }
  [Export] private AudioStreamPlayer AudioPlayer { get; set; }

  [Export] private PackedScene PlayerControllerScene { get; set; }
  [Export] private PackedScene PlayerCharacterScene { get; set; }
  [Export] private PackedScene AIControllerScene { get; set; }
  [Export] private PackedScene HUDScene { get; set; }

  [Export] private Node StateChartNode { get; set; }

  private StateChart StateChart { get; set; }

  private MatchComponent MatchComponent { get; set; }
  private HOBPlayerManagmentComponent PlayerManagmentComponent { get; set; }

  private GameBoard GameBoard => GetGameState().GameBoard;
  public override void _EnterTree() {
    base._EnterTree();

    GetGameState().GameBoard = GameInstance.GetWorld().CurrentLevel.GetChildByType<GameBoard>();

    PlayerManagmentComponent = GetGameModeComponent<HOBPlayerManagmentComponent>();
    MatchComponent = GetGameModeComponent<MatchComponent>();
    StateChart = StateChart.Of(StateChartNode);

    PlayerManagmentComponent.PlayerSpawned += (playerState) => MatchComponent.OnPlayerSpawned(playerState as IMatchPlayerState);

    GameBoard.GridCreated += OnGridCreated;

    GetMatchEvents().GameEnded += MatchEndMenu.OnGameEnd;
    GetMatchEvents().GameEnded += (_) => {
      StateChart.SendEvent("match_end");
    };
  }

  public override void _ExitTree() {
    base._ExitTree();

    GameInstance.SetPause(false);
  }

  public override void _Ready() {
    base._Ready();

    PauseMenu.ResumeEvent += Resume;
    PauseMenu.MainMenuEvent += OnMainMenu;
    PauseMenu.QuitEvent += OnQuit;

    GameBoard.Init();
  }

  public void Pause() {
    StateChart.SendEvent("pause");
  }

  public void Resume() {
    StateChart.SendEvent("resume");
  }

  public bool IsCurrentTurn(IMatchController controller) {
    return MatchComponent.IsCurrentTurn(controller);
  }

  public IMatchEvents GetMatchEvents() => MatchComponent;
  public IEntityManagment GetEntityManagment() => MatchComponent;

  public override HOBGameState GetGameState() => base.GetGameState() as HOBGameState;

  protected override GameState CreateGameState() => new HOBGameState();

  private void OnPausedStateEntered() {
    PauseMenu.Show();
    GameInstance.SetPause(true);
  }
  private void OnPausedStateExited() {
    PauseMenu.Hide();
    GameInstance.SetPause(false);
  }

  private void OnInMatchStateEntered() {
    MatchComponent.OnGameStarted();

    AudioPlayer.Play();

    MatchComponent.TurnStarted += CheckWinCondition;

    foreach (var entity in GetEntityManagment().GetEntities()) {
      entity.OwnerControllerChanged += CheckWinCondition;
    }
  }

  private void OnInMatchStateExited() {
    MatchComponent.TurnStarted -= CheckWinCondition;
  }

  private void OnMatchEndedStateEntered() {
    GameInstance.SetPause(true);
  }

  private void OnMainMenu() {

    // FIXME: normal open level waits some time before unloading
    GameInstance.GetWorld().OpenLevel("main_menu_level");
  }

  private void OnQuit() => GameInstance.QuitGame();

  private void OnGridCreated() {
    PlayerManagmentComponent.SpawnPlayerDeferred(new(PlayerControllerScene, new HOBPlayerState(), "Player", HUDScene, PlayerCharacterScene));
    PlayerManagmentComponent.SpawnPlayerDeferred(new(AIControllerScene, new HOBPlayerState(), "AI", null, null));

    StateChart.CallDeferred(StateChart.MethodName.SendEvent, "match_start");
  }

  private void CheckWinCondition() {
    var alivePlayers = new List<IMatchController>();
    var eliminatedPlayers = new List<IMatchController>();

    foreach (var player in GetGameState().PlayerArray) {
      var controller = player.GetController<IMatchController>();
      var entities = GetEntityManagment().GetOwnedEntites(controller);

      if (entities.Any(e => e.TryGetTrait<EntityProducerTrait>(out _))) {
        alivePlayers.Add(controller);
      }
      else {
        eliminatedPlayers.Add(controller);
      }
    }

    if (eliminatedPlayers.Count > 0) {
      MatchComponent.TriggerGameEnd(alivePlayers[0]);
    }
  }
}
