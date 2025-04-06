namespace GameplayFramework;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public sealed partial class World : Node {
  [Signal]
  public delegate void LevelLoadedEventHandler(Level level);

  private Level? CurrentLevel { get; set; }
  public bool LoadingLevel { get; private set; }

  private string? _loadedLevelPath;
  private LoadingScreen? _loadingScreen;

  public override void _Ready() {
    LevelLoaded += (level) => {
      if (_loadingScreen != null) {
        _loadingScreen.QueueFree();
        _loadingScreen = null;
      }
    };
  }

  public IGameMode GetGameMode() => GetCurrentLevel().GameMode!;
  public Level GetCurrentLevel() => CurrentLevel!;

  public async Task OpenLevel(string levelName) {
    if (IsInstanceValid(CurrentLevel) && CurrentLevel != null) {
      await CurrentLevel.UnLoad();
    }

    var levelPath = GameInstance.Instance!.LevelsDirectoryPath + "/" + levelName + ".tscn";
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
