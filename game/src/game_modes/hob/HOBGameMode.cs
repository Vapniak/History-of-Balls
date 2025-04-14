namespace HOB;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AudioManager;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;
using GodotStateCharts;
using HOB.GameEntity;

[GlobalClass]
public partial class HOBGameMode : GameMode {
  [Export] private MatchEndMenu? MatchEndMenu { get; set; }
  [Export] private PauseMenu? PauseMenu { get; set; }
  [Export] private PlayerAttributeSet? PlayerAttributeSet { get; set; }
  [Export] public Array<EntityIcon>? EntityIcons { get; private set; }

  [Export] private MatchData? MatchData { get; set; }

  [Export] private PackedScene? PlayerControllerScene { get; set; }
  [Export] private PackedScene? PlayerCharacterScene { get; set; }
  [Export] private PackedScene? AIControllerScene { get; set; }
  [Export] private PackedScene? HUDScene { get; set; }

  [Export] private Node? StateChartNode { get; set; }

  private StateChart? StateChart { get; set; }

  [Export] private MatchComponent MatchComponent { get; set; } = default!;
  [Export] private HOBPlayerManagmentComponent PlayerManagmentComponent { get; set; } = default!;

  private GameBoard GameBoard => GetGameState().GameBoard;

  private double _savedTimeScale;

  public override void _EnterTree() {
    base._EnterTree();

    MusicManager.Instance.Play("music", "medival", autoLoop: true);

    GetGameState().GameBoard = GameInstance.GetWorld().GetCurrentLevel().GetChildByType<GameBoard>();

    StateChart = StateChart.Of(StateChartNode!);

    PlayerManagmentComponent!.PlayerSpawned += (playerState) => MatchComponent!.OnPlayerSpawned(playerState as IMatchPlayerState);

    GameBoard.GridCreated += () => CallDeferred(MethodName.OnGridCreated);
  }

  public override void _ExitTree() {
    base._ExitTree();

    Engine.TimeScale = 1;
    GameInstance.SetPause(false);
  }

  public override void _Ready() {
    base._Ready();

    if (PauseMenu != null) {
      PauseMenu.ResumeEvent += Resume;
      PauseMenu.MainMenuEvent += OnMainMenu;
      PauseMenu.QuitEvent += OnQuit;
    }

    if (MatchData?.Map != null) {
      GameBoard.Init(MatchData.Map);
    }
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


  public Texture2D? GetIconFor(Entity entity) {
    return EntityIcons?.FirstOrDefault(i => i.EntityType != null && entity.AbilitySystem.OwnedTags.HasExactTag(i.EntityType), null)?.Icon;
  }

  public Texture2D? GetIconFor(EntityData entityData) {
    return EntityIcons?.FirstOrDefault(i => i.EntityType != null && entityData.Tags.HasExactTag(i.EntityType), null)?.Icon;
  }

  public IMatchEvents GetMatchEvents() => MatchComponent!;
  public IEntityManagment GetEntityManagment() => MatchComponent!;

  public ITurnManagment GetTurnManagment() => MatchComponent!;

  public override HOBGameState GetGameState() => base.GetGameState() as HOBGameState;

  protected override GameState CreateGameState() => new HOBGameState();

  private void OnPausedStateEntered() {
    PauseMenu.Show();
    _savedTimeScale = Engine.TimeScale;
    Engine.TimeScale = 1;
    GameInstance.SetPause(true);
  }
  private void OnPausedStateExited() {
    PauseMenu.Hide();
    GameInstance.SetPause(false);
    Engine.TimeScale = _savedTimeScale;
  }

  private void OnInMatchStateEntered() {
    MatchComponent.OnGameStarted();

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
    _ = InitGame();
  }

  private async Task InitGame() {
    if (MatchData == null) {
      return;
    }

    foreach (var playerData in MatchData.PlayerSpawnDatas) {
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

        var state = new HOBPlayerState(PlayerAttributeSet, playerData.ProducableEntities, playerData.Entities) {
          Country = playerData.Country
        };

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
              MatchComponent.AddEntityOnClosestAvailableCell(entitySpawn.EntityData, new HexGridMap.OffsetCoord(coord.X, coord.Y), controller == null ? null : controller as IMatchController);
              pitch += 0.1f;
            }
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

        if (entities.Any(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureCity)))) {
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
