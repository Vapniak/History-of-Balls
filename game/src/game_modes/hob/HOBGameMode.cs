namespace HOB;

using System.Text.RegularExpressions;
using GameplayFramework;
using Godot;

[GlobalClass]
public partial class HOBGameMode : GameMode {
  [Export] private PackedScene PlayerControllerScene { get; set; }
  [Export] private PackedScene PlayerCharacterScene { get; set; }
  [Export] private PackedScene AIControllerScene { get; set; }
  [Export] private PackedScene HUDScene { get; set; }


  private PauseComponent PauseComponent { get; set; }
  private HOBPlayerManagmentComponent PlayerManagmentComponent { get; set; }
  private MatchComponent MatchComponent { get; set; }

  public override void _EnterTree() {
    base._EnterTree();

    GetGameState().GameBoard = GameInstance.GetWorld().CurrentLevel.GetChildByType<GameBoard>();

    PauseComponent = GetGameModeComponent<PauseComponent>();

    PlayerManagmentComponent = GetGameModeComponent<HOBPlayerManagmentComponent>();

    MatchComponent = GetGameModeComponent<MatchComponent>();

    PlayerManagmentComponent.PlayerSpawned += MatchComponent.OnPlayerSpawned;
    GetGameState().GameBoard.GridCreated += OnStartGame;
  }

  public override void _ExitTree() {
    base._ExitTree();

    if (GetGameState().PauseGame) {
      GameInstance.SetPause(false);
    }
  }

  public override void _Ready() {
    base._Ready();

    PauseComponent.GetPauseMenu().ResumeEvent += OnResume;
    PauseComponent.GetPauseMenu().MainMenuEvent += OnMainMenu;
    PauseComponent.GetPauseMenu().QuitEvent += OnQuit;

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

  public override HOBGameState GetGameState() => base.GetGameState() as HOBGameState;

  protected override GameState CreateGameState() => new HOBGameState();

  private void OnResume() {
    PauseComponent.Resume();

    foreach (var player in GetGameState().PlayerArray) {
      if (player.GetController() is PlayerController playerController) {
        playerController.GetHUD().Show();
      }
    }
  }
  private void OnMainMenu() {
    GameInstance.GetWorld().OpenLevelThreaded("main_menu_level");
  }

  private void OnQuit() => GameInstance.QuitGame();

  private void OnStartGame() {
    GetGameState().Init();

    PlayerManagmentComponent.SpawnPlayerDeferred(new(PlayerControllerScene, new HOBPlayerState(), "Player", HUDScene, PlayerCharacterScene));
    PlayerManagmentComponent.SpawnPlayerDeferred(new(AIControllerScene, new HOBPlayerState(), "AI", null, null));

    MatchComponent.OnGameStarted();
  }
}
