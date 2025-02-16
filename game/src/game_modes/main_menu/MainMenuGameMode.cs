namespace HOB;

using Godot;
using GameplayFramework;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  [Export] private PackedScene _settingsScene;
  [Export] private PackedScene _loadingScreenScene;

  private SettingsMenu _settings;

  protected override GameState CreateGameState() => new MainMenuGameState();

  public override void _Process(double delta) {
    base._Process(delta);


  }

  public void StartGame() => GameInstance.GetWorld().OpenLevelThreaded("test_level", _loadingScreenScene);

  public void OpenSettings() {
    _settings = _settingsScene.Instantiate<SettingsMenu>();
    AddChild(_settings);
    _settings.Closed += CloseSettings;
  }

  public void CloseSettings() {
    _settings.Closed -= CloseSettings;
    _settings.QueueFree();
  }

  public void Quit() => GameInstance.QuitGame();
}
