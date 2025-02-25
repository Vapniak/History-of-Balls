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

  private void OnInMatchStateEntered() {
    MatchComponent.OnGameStarted();

    GetGameState().TurnEndedEvent += CheckWinCondition;
  }

  private void OnInMatchStateExited() {
    GetGameState().TurnEndedEvent -= CheckWinCondition;
  }

  private void OnMatchEndedStateEntered() {
    GetGameState().Pause();
    MatchEndMenu.Show();
  }

  private void OnMainMenu() {

    // FIXME: normal open level waits some time before unloading
    GameInstance.GetWorld().OpenLevelThreaded("main_menu_level");
  }

  private void OnQuit() => GameInstance.QuitGame();

  private void OnGridCreated() {
    GetGameState().Init();

    PlayerManagmentComponent.SpawnPlayerDeferred(new(PlayerControllerScene, new HOBPlayerState(), "Player", HUDScene, PlayerCharacterScene));
    PlayerManagmentComponent.SpawnPlayerDeferred(new(AIControllerScene, new HOBPlayerState(), "AI", null, null));

    StateChart.CallDeferred(StateChart.MethodName.SendEvent, "match_start");
  }

  private void CheckWinCondition() {
    var player = GetGameState().PlayerArray[GetGameState().CurrentPlayerIndex];
    var controller = player.GetController<IMatchController>();

    var board = GetGameState().GameBoard;
    if (board.GetOwnedEntities(controller).Length == 0) {
      StateChart.SendEvent("match_end");
    }
  }
}
