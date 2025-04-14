namespace HOB;

using GameplayFramework;
using Godot;
using System;

public partial class MainMenu : Control {

  private void OnStartGamePressed() {
    GameInstance.GetGameMode<MainMenuGameMode>().StartGame();
  }
  private void OnQuitButtonPressed() {
    GameInstance.QuitGame();
  }
}
