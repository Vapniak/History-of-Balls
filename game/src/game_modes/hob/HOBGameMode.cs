namespace HOB;

using GameplayFramework;
using Godot;
using GodotStateCharts;

[GlobalClass]
public partial class HOBGameMode : GameMode {
  [Export] private MatchEndMenu MatchEndMenu { get; set; }

  [Export] private PackedScene PlayerControllerScene { get; set; }
  [Export] private PackedScene PlayerCharacterScene { get; set; }
  [Export] private PackedScene AIControllerScene { get; set; }
  [Export] private PackedScene HUDScene { get; set; }

  [Export] private Node StateChartNode { get; set; }

  private StateChart StateChart { get; set; }

  private PauseComponent PauseComponent { get; set; }
  private HOBPlayerManagmentComponent PlayerManagmentComponent { get; set; }
  private MatchComponent MatchComponent { get; set; }

  public override void _EnterTree() {
    base._EnterTree();

    GetGameState().GameBoard = GameInstance.GetWorld().CurrentLevel.GetChildByType<GameBoard>();


    PauseComponent = GetGameModeComponent<PauseComponent>();
    PlayerManagmentComponent = GetGameModeComponent<HOBPlayerManagmentComponent>();
    MatchComponent = GetGameModeComponent<MatchComponent>();
    StateChart = StateChart.Of(StateChartNode);


    PlayerManagmentComponent.PlayerSpawned += (playerState) => MatchComponent.OnPlayerSpawned(playerState as IMatchPlayerState);
    GetGameState().GameBoard.EntityRemoved += (entity) => {
      if (entity.TryGetOwner(out var owner)) {
        if (GetGameState().GameBoard.GetOwnedEntities(owner).Length == 0) {
          // FIXME: match end is called when exiting tree so the game is stopped
          //StateChart.SendEvent("match_end");
        }
      }
    };

    GetGameState().GameBoard.GridCreated += OnGridCreated;
  }

  public override void _ExitTree() {
    base._ExitTree();

    GetGameState().Resume();
  }

  public override void _Ready() {
    base._Ready();

    PauseComponent.GetPauseMenu().ResumeEvent += GetGameState().Resume;
    PauseComponent.GetPauseMenu().MainMenuEvent += OnMainMenu;
    PauseComponent.GetPauseMenu().QuitEvent += OnQuit;

    GetGameState().GameBoard.Init();
  }

  public void Pause() {
    StateChart.SendEvent("pause");
  }

  public void Resume() {
    StateChart.SendEvent("resume");
  }

  public override HOBGameState GetGameState() => base.GetGameState() as HOBGameState;

  protected override GameState CreateGameState() => new HOBGameState();

  private void OnPausedStateEntered() {
    PauseComponent.ShowPauseMenu();
  }
  private void OnPausedStateExited() {
    PauseComponent.HidePauseMenu();
  }

  private void OnMatchEndedStateEntered() {
    MatchEndMenu.Show();
    GetGameState().Pause();
  }

  private void OnMainMenu() {
    GameInstance.GetWorld().OpenLevel("main_menu_level");
  }

  private void OnQuit() => GameInstance.QuitGame();

  private void OnGridCreated() {
    GetGameState().Init();

    PlayerManagmentComponent.SpawnPlayerDeferred(new(PlayerControllerScene, new HOBPlayerState(), "Player", HUDScene, PlayerCharacterScene));
    PlayerManagmentComponent.SpawnPlayerDeferred(new(AIControllerScene, new HOBPlayerState(), "AI", null, null));

    StateChart.CallDeferred(StateChart.MethodName.SendEvent, "match_start");
  }
}
