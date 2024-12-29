namespace HOB;

using GameplayFramework;
using Godot;

[GlobalClass]
public partial class PauseComponent : GameModeComponent, IGameModeComponent<IPauseGameState> {
  [Export] public PackedScene PauseMenuScene { get; private set; }

  private PauseMenu _pauseMenu;

  private bool _pauseMenuShown;

  public override void Init() {
    base.Init();

    _pauseMenu = PauseMenuScene.Instantiate<PauseMenu>();
    _pauseMenu.Visible = false;

    AddChild(_pauseMenu);
  }

  public void Resume() {
    _pauseMenu.Visible = false;
    if (GetGameState().PauseGame) {
      Game.PauseGame();
    }

    _pauseMenuShown = false;
  }
  public void Pause() {
    if (_pauseMenuShown) {
      return;
    }
    _pauseMenu.Visible = true;
    _pauseMenuShown = true;

    if (GetGameState().PauseGame) {
      Game.PauseGame();
    }
  }

  public IPauseGameState GetGameState() => GameState as IPauseGameState;
  public IPauseMenu GetPauseMenu() => _pauseMenu;
}
