namespace HOB;

using Godot;
using GameplayFramework;
using AudioManager;

[GlobalClass]
public partial class MainMenuGameMode : GameMode {
  [Export] private PackedScene _settingsScene;
  [Export] private PackedScene _tutorialScene;
  [Export] private PackedScene _loadingScreenScene;


  private SettingsMenu _settings;
  private TutorialPanel _tutorial;

  protected override GameState CreateGameState() => new MainMenuGameState();

  public override void _EnterTree() {
    base._EnterTree();
  }

  public void StartGame() {
    MusicManager.Instance.Stop(1);
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
