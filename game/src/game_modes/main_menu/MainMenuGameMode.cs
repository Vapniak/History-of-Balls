namespace HOB;

using Godot;
using GameplayFramework;
using AudioManager;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  [Export] private PackedScene _loadingScreenScene = default!;

  protected override GameState CreateGameState() => new MainMenuGameState();

  public override void _EnterTree() {
    base._EnterTree();

    MusicManager.Instance.Play("music", "main_menu", 2, true);
  }

  public void StartGame() {
    _ = GameInstance.GetWorld().OpenLevelThreaded("test_level", _loadingScreenScene);
  }

  public void Quit() => GameInstance.QuitGame();
}
