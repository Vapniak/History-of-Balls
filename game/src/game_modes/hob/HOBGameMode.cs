namespace HOB;

using System.Collections.Generic;
using GameplayFramework;
using GameplayTags;
using Godot;
using GodotStateCharts;

[GlobalClass]
public partial class HOBGameMode : GameMode {
  [Export] private MatchEndMenu MatchEndMenu { get; set; }
  [Export] private PauseMenu PauseMenu { get; set; }
  [Export] private AudioStreamPlayer AudioPlayer { get; set; }
  [Export] private PlayerAttributeSet? PlayerAttributeSet { get; set; }

  [Export] private MatchData? MatchData { get; set; }

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

    GameBoard.GridCreated += () => CallDeferred(MethodName.OnGridCreated);
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

    if (MatchData?.Map != null) {
      GameBoard.Init(MatchData.Map);
    }
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

    MatchComponent.MatchEvent += CheckWinCondition;

    foreach (var entity in GetEntityManagment().GetEntities()) {
      entity.OwnerControllerChanged += () => CheckWinCondition(null);
    }
  }

  private void OnInMatchStateExited() {
    MatchComponent.MatchEvent -= CheckWinCondition;
  }

  private void OnInMatchPlayingStateProcess(float delta) {
    GetGameState().GameTimeMSec += (ulong)(delta * 1000f);
  }


  private void OnMatchEndedStateEntered() {
    GameInstance.SetPause(true);
  }

  private void OnMainMenu() {

    // FIXME: normal open level waits some time before unloading
    _ = GameInstance.GetWorld().OpenLevel("main_menu_level");
  }

  private void OnQuit() => GameInstance.QuitGame();

  private void OnGridCreated() {
    if (MatchData == null) {
      return;
    }

    foreach (var playerData in MatchData.PlayerSpawnDatas) {
      var state = new HOBPlayerState(PlayerAttributeSet);

      Controller? controller = null;
      if (playerData.PlayerType == PlayerType.Player) {
        controller = PlayerControllerScene.Instantiate<Controller>();
        var hud = HUDScene.Instantiate<HUD>();
        var characterNode = PlayerCharacterScene.Instantiate<Node>();
        PlayerManagmentComponent.SpawnPlayer(new(controller, state, "Player", hud, characterNode));
      }
      else if (playerData.PlayerType == PlayerType.AI) {
        controller = AIControllerScene.Instantiate<Controller>();
        PlayerManagmentComponent.SpawnPlayer(new(controller, state, "AI"));
      }

      state.Country = playerData.Country;

      if (playerData.Entities != null) {
        foreach (var entitySpawn in playerData.Entities) {
          if (entitySpawn?.EntityData != null) {
            MatchComponent.AddEntityOnClosestAvailableCell(entitySpawn.EntityData, new HexGridMap.OffsetCoord(entitySpawn.Col, entitySpawn.Row), controller as IMatchController);
          }
        }
      }
    }

    StateChart.CallDeferred(StateChart.MethodName.SendEvent, "match_start");
  }

  private void CheckWinCondition(Tag? tag) {
    if (tag == TagManager.GetTag(HOBTags.EventTurnStarted) || tag == null) {
      var alivePlayers = new List<IMatchController>();
      var eliminatedPlayers = new List<IMatchController>();

      foreach (var player in GetGameState().PlayerArray) {
        var controller = player.GetController<IMatchController>();
        var entities = GetEntityManagment().GetOwnedEntites(controller);

        if (entities.Length != 0) {
          alivePlayers.Add(controller);
        }
        else {
          eliminatedPlayers.Add(controller);
        }
      }

      if (eliminatedPlayers.Count > 0) {
        MatchComponent.TriggerGameEnd(alivePlayers[0]);

        StateChart.SendEvent("match_end");
        MatchEndMenu.OnGameEnd(alivePlayers[0]);
      }
    }
  }
}
