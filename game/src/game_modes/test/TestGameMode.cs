namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class TestGameMode : GameMode {
  private PauseComponent PauseComponent { get; set; }
  private TestPlayerManagmentComponent PlayerManagmentComponent { get; set; }
  private MatchComponent MatchComponent { get; set; }

  public override void Init() {
    base.Init();

    GetGameState().GameBoard = Game.GetWorld().CurrentLevel.GetChildByType<GameBoard>();

    PauseComponent = GetGameModeComponent<PauseComponent>();
    PauseComponent.GetPauseMenu().Resume += OnResume;
    PauseComponent.GetPauseMenu().MainMenu += OnMainMenu;
    PauseComponent.GetPauseMenu().Quit += OnQuit;

    PlayerManagmentComponent = GetGameModeComponent<TestPlayerManagmentComponent>();

    MatchComponent = GetGameModeComponent<MatchComponent>();

    PlayerManagmentComponent.PlayerSpawned += MatchComponent.OnPlayerSpawned;
    GetGameState().GameBoard.GridCreated += OnStartGame;

    GetGameState().GameBoard.Init();
  }

  public override void _Process(double delta) {
    base._Process(delta);

    if (Input.IsActionJustPressed(BuiltinInputActions.UICancel)) {
      PauseComponent.Pause();
      PlayerManagmentComponent.GetLocalPlayer().GetController<PlayerController>().GetHUD().Hide();
    }
  }

  public override TestGameState GetGameState() => base.GetGameState() as TestGameState;

  protected override GameState CreateGameState() => new TestGameState();

  private void OnResume() {
    PauseComponent.Resume();
    PlayerManagmentComponent.GetLocalPlayer().GetController<PlayerController>().GetHUD().Show();
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
