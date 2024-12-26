namespace HOB;

using Godot;
using GameplayFramework;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  [Export] private PackedScene _startLevel;
  public void StartGame() {
    // FIXME: temp level selection
    // TODO: level selection screen and transitions
    var level = _startLevel.InstantiateOrNull<Level>();
    Game.GetWorld().OpenLevel(level);
  }

  public void Settings() {

  }

  public void Quit() {
    Game.Instance.QuitGame();
  }
}
