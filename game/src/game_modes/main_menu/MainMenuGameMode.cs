namespace HOB;

using Godot;
using GameplayFramework;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  [Export] private PackedScene _settingsScene;
  [Export] private PackedScene _tutorialScene;
  [Export] private PackedScene _loadingScreenScene;


  private SettingsMenu _settings;
  private TutorialPanel _tutorial;

  protected override GameState CreateGameState() => new MainMenuGameState();

  public void StartGame() {
    GameInstance.GetWorld().OpenLevel("test_level");
  }

  public void OpenSettings() {
    _settings = _settingsScene.Instantiate<SettingsMenu>();
    AddChild(_settings);
    _settings.Closed += CloseSettings;
  }

  public void OpenTutorial() {
    _tutorial = _tutorialScene.Instantiate<TutorialPanel>();
    AddChild(_tutorial);
    _tutorial.Closed += CloseTutorial;
  }

  public void CloseTutorial() {
    _tutorial.QueueFree();
  }
  public void CloseSettings() {
    _settings.QueueFree();
  }

  public void Quit() => GameInstance.QuitGame();
}
