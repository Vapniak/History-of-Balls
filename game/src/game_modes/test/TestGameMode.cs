namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class TestGameMode : GameMode {
  private PauseComponent PauseComponent { get; set; }
  public override void Init() {
    base.Init();

    GetGameState<TestGameState>().GameBoard = Game.GetWorld().CurrentLevel.GetChildByType<GameBoard>();
    PauseComponent = GetGameModeComponent<PauseComponent>();
    PauseComponent.GetPauseMenu().Resume += OnResume;
    PauseComponent.GetPauseMenu().MainMenu += OnMainMenu;
    PauseComponent.GetPauseMenu().Quit += OnQuit;
  }

  public override void _Process(double delta) {
    base._Process(delta);

    if (Input.IsActionJustPressed(BuiltinInputActions.UICancel)) {
      GetGameModeComponent<PauseComponent>().Pause();
    }
  }

  protected override GameState CreateGameState() => new TestGameState();

  private void OnResume() {
    PauseComponent.Resume();
  }
  private void OnMainMenu() {
    OnResume();
    Game.GetWorld().OpenLevel("main_menu_level");
  }

  private void OnQuit() {
    Game.Instance.QuitGame();
  }
}
