namespace HOB;

using GameplayFramework;
using Godot;
using System;

public partial class MainMenu : Control {
  private void OnStartButtonPressed() {
    Game.GetGameMode<MainMenuGameMode>().StartGame();
  }

  private void OnSettingsButtonPressed() {
    Game.GetGameMode<MainMenuGameMode>().Settings();
  }

  private void OnQuitButtonPressed() {
    Game.GetGameMode<MainMenuGameMode>().Quit();
  }
}
