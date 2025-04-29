namespace HOB;

using System.Diagnostics;
using System.Threading.Tasks;
using AudioManager;
using GameplayFramework;
using Godot;
using GodotStateCharts;
using HexGridMap;
using WidgetSystem;

[GlobalClass]
public partial class HOBGameMode : GameMode {
  [Export] private PlayerAttributeSet? PlayerAttributeSet { get; set; }
  [Export] private PackedScene? PlayerControllerScene { get; set; }
  [Export] private PackedScene? PlayerCharacterScene { get; set; }
  [Export] private PackedScene? AIControllerScene { get; set; }
  [Export] private PackedScene? HUDScene { get; set; }

  [Export] private Node? StateChartNode { get; set; }

  private StateChart? StateChart { get; set; }

  [Export] private MatchComponent MatchComponent { get; set; } = default!;
  [Export] private HOBPlayerManagmentComponent PlayerManagmentComponent { get; set; } = default!;

  public MissionData MissionData { get; set; } = default!;

  private GameBoard GameBoard => GetGameState().GameBoard;

  private double _savedTimeScale;

  public override void _EnterTree() {
    base._EnterTree();

    MusicManager.Instance.Stop(1);
  }

  public override void _ExitTree() {
    base._ExitTree();

    WidgetManager.Instance.PopAllWidgets();
    Engine.TimeScale = 1;
    GameInstance.SetPause(false);
  }

  public override void _Ready() {
    base._Ready();

    GetGameState().GameBoard = GameInstance.GetWorld().GetCurrentLevel().GetChildByType<GameBoard>();

    StateChart = StateChart.Of(StateChartNode!);

    PlayerManagmentComponent!.PlayerSpawned += (playerState) => MatchComponent!.OnPlayerSpawned(playerState as IMatchPlayerState);

    GameBoard.GridCreated += () => CallDeferred(MethodName.OnGridCreated);

    if (MissionData?.Map != null) {
      GameBoard.Init(MissionData);
    }

    SoundManager.Instance.Play("sound", "intro_map");
  }

  public void Pause() {
    StateChart?.SendEvent("pause");
  }

  public void Resume() {
    StateChart?.SendEvent("resume");
  }

  public bool IsCurrentTurn(IMatchController controller) {
    return MatchComponent.IsCurrentTurn(controller);
  }

  public IMatchEvents GetMatchEvents() => MatchComponent!;
  public IEntityManagment GetEntityManagment() => MatchComponent!;

  public ITurnManagment GetTurnManagment() => MatchComponent!;

  public override HOBGameState GetGameState() => base.GetGameState() as HOBGameState;

  protected override GameState CreateGameState() => new HOBGameState();

  private void OnPausedStateEntered() {
    WidgetManager.Instance.PushWidget<PauseMenuWidget>(menu => {
      menu.ResumeEvent += Resume;
      menu.MainMenuEvent += () => _ = OnMainMenu();
      menu.QuitEvent += OnQuit;
    });

    _savedTimeScale = Engine.TimeScale;
    Engine.TimeScale = 1;
    GameInstance.SetPause(true);
  }
  private void OnPausedStateExited() {
    GameInstance.SetPause(false);
    Engine.TimeScale = _savedTimeScale;
  }

  protected virtual void OnInMatchStateEntered() {
    MatchComponent.OnGameStarted();
    MusicManager.Instance.Play("music", "medival", autoLoop: true);
  }

  protected virtual void OnInMatchStateExited() {

  }

  private void OnInMatchPlayingStateProcess(float delta) {
    GetGameState().GameTimeMSec += (ulong)(delta * 1000f);
  }


  private void OnMatchEndedStateEntered() {
    GameInstance.SetPause(true);
  }

  private async Task OnMainMenu() {
    await GameInstance.GetWorld().OpenLevel("main_menu_level");
  }

  private void OnQuit() => GameInstance.QuitGame();

  private void OnGridCreated() {
    _ = InitGame();
  }

  private async Task InitGame() {
    if (MissionData == null) {
      return;
    }

    foreach (var playerData in MissionData.PlayerSpawnDatas) {
      Controller? controller = null;
      if (playerData.PlayerType != PlayerType.None) {
        if (PlayerAttributeSet == null) {
          Debug.Assert(false, "Player attribute set cannot be null");
          return;
        }

        if (playerData.ProducableEntities == null) {
          Debug.Assert(false, "Producable entities cannot be null");
          return;
        }

        var state = new HOBPlayerState(PlayerAttributeSet, playerData.ProducableEntities.ProducableEntities, playerData.OwnedEntities.Entities, playerData.Country);

        if (playerData.PlayerType == PlayerType.Player) {
          controller = PlayerControllerScene.Instantiate<Controller>();
          var hud = HUDScene.Instantiate<HUD>();
          var characterNode = PlayerCharacterScene.Instantiate<Node>();
          PlayerManagmentComponent.SpawnPlayer(new(controller, state, "Player", hud, characterNode));
        }
        else if (playerData.PlayerType == PlayerType.AI) {
          var ai = AIControllerScene.Instantiate<AIController>();
          if (playerData.AIProfile != null) {
            ai.Profile = playerData.AIProfile;
          }

          controller = ai;
          PlayerManagmentComponent.SpawnPlayer(new(controller, state, "AI"));
        }

      }

      await Task.Delay(500);
      if (playerData.SpawnedEntities != null) {
        foreach (var entitySpawn in playerData.SpawnedEntities) {
          var pitch = 5f;
          if (entitySpawn?.EntityData != null && entitySpawn.SpawnAt != null) {
            foreach (var coord in entitySpawn.SpawnAt) {
              await Task.Delay(50);
              MatchComponent.AddEntityOnClosestAvailableCell(entitySpawn.EntityData, new OffsetCoord(coord.X, coord.Y), controller == null ? null : controller as IMatchController, new Vector3(0, Mathf.Pi, 0));
              pitch += 0.1f;
            }
          }
        }
      }
    }

    StateChart.CallDeferred(StateChart.MethodName.SendEvent, "match_start");
  }

  protected void EndGame(IMatchController winner) {
    MatchComponent.TriggerGameEnd(winner);

    StateChart.SendEvent("match_end");
    WidgetManager.Instance.PushWidget<MatchEndMenuWidget>(menu => {
      menu.OnGameEnd(winner);
    });
  }

  // protected virtual IMatchController? CheckWinner() {
  //   var alivePlayers = new List<IMatchController>();
  //   var eliminatedPlayers = new List<IMatchController>();

  //   foreach (var player in GetGameState().PlayerArray) {
  //     var controller = player.GetController<IMatchController>();
  //     var entities = GetEntityManagment().GetOwnedEntites(controller);

  //     if (entities.Any(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureCity)))) {
  //       alivePlayers.Add(controller);
  //     }
  //     else {
  //       eliminatedPlayers.Add(controller);
  //     }
  //   }

  //   if (eliminatedPlayers.Count > 0) {
  //     return alivePlayers.FirstOrDefault();
  //   }
  //   else {
  //     return null;
  //   }
  // }
}
