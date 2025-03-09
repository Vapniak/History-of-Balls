namespace GameplayFramework;

using System.Threading.Tasks;
using Godot;
using Godot.Collections;

[GlobalClass]
public sealed partial class World : Node {
  [Signal]
  public delegate void LevelLoadedEventHandler(Level level);

  public Level CurrentLevel { get; private set; }
  public bool LoadingLevel { get; private set; }

  private string _loadedLevelPath;
  private LoadingScreen _loadingScreen;

  public override void _Ready() {
    LevelLoaded += (level) => {
      if (IsInstanceValid(_loadingScreen)) {
        _loadingScreen.QueueFree();
        _loadingScreen = null;
      }
    };
  }

  public GameMode GetGameMode() => CurrentLevel.GameMode;

  public async Task OpenLevel(string levelName) {
    if (IsInstanceValid(CurrentLevel)) {
      await CurrentLevel.UnLoad();
    }

    var levelPath = GameInstance.Instance.LevelsDirectoryPath + "/" + levelName + ".tscn";
    var level = ResourceLoader.Load<PackedScene>(levelPath).Instantiate<Level>();
    SwitchLevel(level);
  }


  private void SwitchLevel(Level level) {
    CurrentLevel = level;

    CurrentLevel.TreeEntered += async () => {
      EmitSignal(SignalName.LevelLoaded, level);
      await CurrentLevel.Load();
    };
    AddChild(CurrentLevel);
  }
}
