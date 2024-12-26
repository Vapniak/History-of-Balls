namespace HOB;

using Godot;
using GameplayFramework;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  [Export] private PackedScene _startLevelScene;
  [Export] private PackedScene _settingsScene;

  private SettingsMenu _settings;
  public void StartGame() {
    // FIXME: temp level selection
    // TODO: level selection screen and transitions
    var level = _startLevelScene.InstantiateOrNull<Level>();
    Game.GetWorld().OpenLevel(level);
  }

  public void OpenSettings() {
    _settings = _settingsScene.Instantiate<SettingsMenu>();
    AddChild(_settings);
    _settings.Closed += CloseSettings;
  }

  public void CloseSettings() {
    _settings.QueueFree();
    _settings.Closed -= CloseSettings;
  }

  public void Quit() {
    Game.Instance.QuitGame();
  }
}
