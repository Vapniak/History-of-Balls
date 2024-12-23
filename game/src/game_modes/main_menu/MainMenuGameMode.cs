namespace HOB;

using Godot;
using GameplayFramework;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  public void StartGame() {

  }

  public void Settings() {

  }

  public void Quit() {
    Game.Instance.QuitGame();
  }
}
