namespace GameplayFramework;

using System.Diagnostics;
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

  public async Task OpenLevel(string levelName, IGameModeConfig<GameMode>? config = null) {
    if (IsInstanceValid(CurrentLevel) && CurrentLevel != null) {
      await CurrentLevel.UnLoad();
    }

    var levelPath = GameInstance.Instance!.LevelsDirectoryPath + "/" + levelName + ".tscn";
    var level = ResourceLoader.Load<PackedScene>(levelPath).Instantiate<Level>();
    Debug.Assert(ResourceLoader.Exists(levelPath), $"Level doesnt exist {levelPath}");

    SwitchLevel(level, config);
  }

  public async Task OpenLevelThreaded(string levelName, PackedScene? loadingScreenScene, IGameModeConfig<GameMode>? config = null) {
    if (IsInstanceValid(CurrentLevel) && CurrentLevel != null) {
      await CurrentLevel.UnLoad();
    }

    var levelPath = GameInstance.Instance!.LevelsDirectoryPath + "/" + levelName + ".tscn";
    Debug.Assert(ResourceLoader.Exists(levelPath), $"Level doesnt exist {levelPath}");

    await LoadLevelThreaded(levelPath, loadingScreenScene, config);
  }
  private async Task LoadLevelThreaded(string levelPath, PackedScene? loadingScreenScene, IGameModeConfig<GameMode>? config = null) {
    GD.Print("Level Loading started: " + levelPath);

    _loadingScreen = loadingScreenScene?.InstantiateOrNull<LoadingScreen>();
    if (_loadingScreen != null) {
      GetTree().Root.AddChild(_loadingScreen);
      _loadingScreen.SetProgressBarValue(0.01f);
    }

    var loadError = ResourceLoader.LoadThreadedRequest(levelPath);
    if (loadError != Error.Ok) {
      GD.PrintErr($"Failed to start loading level: {levelPath} (Error: {loadError})");
      CleanupLoadingScreen();
      return;
    }


    var progressArray = new Godot.Collections.Array { 0f };
    var status = ResourceLoader.ThreadLoadStatus.Failed;

    while (true) {
      status = ResourceLoader.LoadThreadedGetStatus(levelPath, progressArray);
      var progress = progressArray[0].As<float>();

      GD.Print($"Load Status: {status}, Progress: {progress}");

      if (status == ResourceLoader.ThreadLoadStatus.Failed) {
        GD.PrintErr($"Failed to load level: {levelPath}");
        CleanupLoadingScreen();
        return;
      }
      else if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
        break;
      }
      else if (status == ResourceLoader.ThreadLoadStatus.InProgress) {
        _loadingScreen?.SetProgressBarValue(Mathf.Max(0.01f, progress));
        await Task.Delay(16);
      }
    }

    _loadingScreen?.SetProgressBarValue(1f);
    await Task.Delay(1000);

    var levelResource = ResourceLoader.LoadThreadedGet(levelPath);
    if (levelResource is PackedScene levelScene) {
      var level = levelScene.Instantiate<Level>();
      SwitchLevel(level, config);
    }
    else {
      GD.PrintErr($"Loaded resource is not a PackedScene: {levelPath}");
      CleanupLoadingScreen();
    }
  }

  private void CleanupLoadingScreen() {
    _loadingScreen?.QueueFree();
    _loadingScreen = null;
  }


  private void SwitchLevel(Level level, IGameModeConfig<GameMode>? config) {
    GD.Print("Switching levels");

    CurrentLevel = level;

    CurrentLevel.TreeEntered += async () => {
      EmitSignal(SignalName.LevelLoaded, level);
      await CurrentLevel.Load(config);
    };
    AddChild(CurrentLevel);
  }
}
