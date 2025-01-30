namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class TestGameMode : GameMode {
  [Export] private PackedScene PlayerControllerScene { get; set; }
  [Export] private PackedScene PlayerCharacterScene { get; set; }
  [Export] private PackedScene AIControllerScene { get; set; }
  [Export] private PackedScene HUDScene { get; set; }


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
    PlayerManagmentComponent.SpawnPlayerDeferred(new(AIControllerScene, new TestPlayerState(), "AI", null, null));
    PlayerManagmentComponent.SpawnPlayerDeferred(new(PlayerControllerScene, new TestPlayerState(), "Player", HUDScene, PlayerCharacterScene));
  }
}
