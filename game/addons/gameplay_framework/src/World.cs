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

  public async Task OpenLevel(string levelName, IGameModeConfig<GameMode>? config = null) {
    if (IsInstanceValid(CurrentLevel) && CurrentLevel != null) {
      await CurrentLevel.UnLoad();
    }

    var levelPath = GameInstance.Instance!.LevelsDirectoryPath + "/" + levelName + ".tscn";
    var level = ResourceLoader.Load<PackedScene>(levelPath).Instantiate<Level>();
    SwitchLevel(level, config);
  }

  public async Task OpenLevelThreaded(string levelName, PackedScene? loadingScreenScene, IGameModeConfig<GameMode>? config = null) {
    if (IsInstanceValid(CurrentLevel) && CurrentLevel != null) {
      await CurrentLevel.UnLoad();
    }

    var levelPath = GameInstance.Instance!.LevelsDirectoryPath + "/" + levelName + ".tscn";
    await LoadLevelThreaded(levelPath, loadingScreenScene, config);
  }

  private async Task LoadLevelThreaded(string levelPath, PackedScene? loadingScreenScene, IGameModeConfig<GameMode>? config = null) {
    _loadingScreen = loadingScreenScene?.InstantiateOrNull<LoadingScreen>();
    if (_loadingScreen != null) {
      GetTree().Root.AddChild(_loadingScreen);
      _loadingScreen.SetProgressBarValue(0);
    }

    var loadState = ResourceLoader.LoadThreadedRequest(levelPath);

    if (loadState != Error.Ok) {
      GD.PrintErr($"Failed to start loading level: {levelPath}");
      _loadingScreen?.QueueFree();
      _loadingScreen = null;
      return;
    }

    var arr = new Godot.Collections.Array();
    var status = ResourceLoader.LoadThreadedGetStatus(levelPath, arr);
    while (status == ResourceLoader.ThreadLoadStatus.InProgress) {
      _loadingScreen?.SetProgressBarValue(arr[0].As<float>());

      await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
      status = ResourceLoader.LoadThreadedGetStatus(levelPath);
    }

    if (status == ResourceLoader.ThreadLoadStatus.Loaded) {
      _loadingScreen?.SetProgressBarValue(1);

      await Task.Delay(1000);

      if (ResourceLoader.LoadThreadedGet(levelPath) is PackedScene levelScene) {
        var level = levelScene.Instantiate<Level>();
        SwitchLevel(level, config);
      }
      else {
        GD.PrintErr($"Failed to instantiate level from: {levelPath}");
        _loadingScreen?.QueueFree();
        _loadingScreen = null;
      }
    }
    else {
      GD.PrintErr($"Failed to load level: {levelPath}");
      _loadingScreen?.QueueFree();
      _loadingScreen = null;
    }
  }



  private void SwitchLevel(Level level, IGameModeConfig<GameMode>? config) {
    CurrentLevel = level;

    CurrentLevel.TreeEntered += async () => {
      EmitSignal(SignalName.LevelLoaded, level);
      await CurrentLevel.Load(config);
    };
    AddChild(CurrentLevel);
  }
}
