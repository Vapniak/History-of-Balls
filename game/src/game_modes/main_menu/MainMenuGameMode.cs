namespace HOB;

using Godot;
using GameplayFramework;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  [Export] private PackedScene _settingsScene;

  private SettingsMenu _settings;

  protected override GameState CreateGameState() => new MainMenuGameState();

  public void StartGame() => Game.GetWorld().OpenLevel("tests_level");

  public void OpenSettings() {
    _settings = _settingsScene.Instantiate<SettingsMenu>();
    AddChild(_settings);
    _settings.Closed += CloseSettings;
  }

  public void CloseSettings() {
    _settings.Closed -= CloseSettings;
    _settings.QueueFree();
  }

  public void Quit() => Game.QuitGame();
}
