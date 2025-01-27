namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class TestGameMode : GameMode {
  [Export] private CanvasLayer LoadingScreen { get; set; }

  private PauseComponent PauseComponent { get; set; }
  private TestPlayerManagmentComponent PlayerManagmentComponent { get; set; }
  private MatchComponent MatchComponent { get; set; }

  public override void _EnterTree() {
    base._EnterTree();

    GetGameState().GameBoard = Game.GetWorld().CurrentLevel.GetChildByType<GameBoard>();

    PauseComponent = GetGameModeComponent<PauseComponent>();

    PlayerManagmentComponent = GetGameModeComponent<TestPlayerManagmentComponent>();

    MatchComponent = GetGameModeComponent<MatchComponent>();

    PlayerManagmentComponent.PlayerSpawned += MatchComponent.OnPlayerSpawned;
    GetGameState().GameBoard.GridCreated += OnStartGame;

    Game.GetWorld().LevelLoaded += OnLevelLoaded;
  }

  public override void _ExitTree() {
    base._ExitTree();

    Game.GetWorld().LevelLoaded -= OnLevelLoaded;
  }

  public override void _Ready() {
    base._Ready();

    PauseComponent.GetPauseMenu().Resume += OnResume;
    PauseComponent.GetPauseMenu().MainMenu += OnMainMenu;
    PauseComponent.GetPauseMenu().Quit += OnQuit;

    GetGameState().GameBoard.Init();
  }

  public override void _Process(double delta) {
    base._Process(delta);


    // TODO: add unpausing on escape
    if (Input.IsActionJustPressed(BuiltinInputActions.UICancel)) {
      PauseComponent.Pause();
      foreach (var player in GetGameState().PlayerArray) {
        if (player.GetController() is PlayerController playerController) {
          playerController.GetHUD().Hide();
        }
      }
    }
  }

  public override TestGameState GetGameState() => base.GetGameState() as TestGameState;

  protected override GameState CreateGameState() => new TestGameState();

  private void OnLevelLoaded(Level level) {
    var timer = new Timer();
    AddChild(timer);
    timer.Timeout += () => {
      LoadingScreen.Visible = false;
      timer.QueueFree();
    };
    timer.Start(1);
  }
  private void OnResume() {
    PauseComponent.Resume();

    foreach (var player in GetGameState().PlayerArray) {
      if (player.GetController() is PlayerController playerController) {
        playerController.GetHUD().Show();
      }
    }
  }
  private void OnMainMenu() {
    OnResume();
    Game.GetWorld().OpenLevel("main_menu_level");
  }

  private void OnQuit() => Game.QuitGame();

  private void OnStartGame() {
    PlayerManagmentComponent.SpawnPlayerDeffered();
  }
}
