namespace HOB;

using GameplayFramework;
using Godot;
using GodotStateCharts;

[GlobalClass]
public partial class HOBGameMode : GameMode {
  [Export] private MatchEndMenu MatchEndMenu { get; set; }
  [Export] private PauseMenu PauseMenu { get; set; }

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

    MatchComponent.TurnStarted += CheckWinCondition;
  }

  private void OnInMatchStateExited() {
    MatchComponent.TurnStarted -= CheckWinCondition;
  }

  private void OnMatchEndedStateEntered() {
    MatchEndMenu.Show();
    GameInstance.SetPause(true);
  }

  private void OnMainMenu() {

    // FIXME: normal open level waits some time before unloading
    GameInstance.GetWorld().OpenLevelThreaded("main_menu_level");
  }

  private void OnQuit() => GameInstance.QuitGame();

  private void OnGridCreated() {
    PlayerManagmentComponent.SpawnPlayerDeferred(new(PlayerControllerScene, new HOBPlayerState(), "Player", HUDScene, PlayerCharacterScene));
    PlayerManagmentComponent.SpawnPlayerDeferred(new(AIControllerScene, new HOBPlayerState(), "AI", null, null));

    StateChart.CallDeferred(StateChart.MethodName.SendEvent, "match_start");
  }

  private void CheckWinCondition() {
    foreach (var player in GetGameState().PlayerArray) {
      var controller = player.GetController<IMatchController>();

      if (GetEntityManagment().GetOwnedEntites(controller).Length == 0) {
        StateChart.SendEvent("match_end");
      }
    }
  }
}
