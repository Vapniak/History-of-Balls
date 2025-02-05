namespace GameplayFramework;

using Godot;
using Godot.Collections;

/// <summary>
/// Base class for all worlds.
/// </summary>
[GlobalClass]
public sealed partial class World : Node {
  [Signal] public delegate void LevelLoadedEventHandler(Level level);

  public Level CurrentLevel { get; private set; }

  public bool LoadingLevel { get; private set; }

  private string _loadedLevelPath;
  private LoadingScreen _loadingScreen;

  public GameMode GetGameMode() => CurrentLevel.GameMode;

  public override void _Ready() {
    LevelLoaded += (level) => {
      if (IsInstanceValid(_loadingScreen)) {
        _loadingScreen.QueueFree();
        _loadingScreen = null;
      }
    };
  }

  /// <summary>
  /// Loads level by its name.
  /// </summary>
  /// <param name="levelName">Name of saved level scene.</param>
  public void OpenLevelThreaded(string levelName, PackedScene loadingScreenScene = null) {
    _loadedLevelPath = Game.Instance.LevelsDirectoryPath + "/" + levelName + ".tscn";
    var error = ResourceLoader.LoadThreadedRequest(_loadedLevelPath, useSubThreads: true);

    if (error == Error.Ok) {
      if (loadingScreenScene != null) {
        _loadingScreen = loadingScreenScene.Instantiate<LoadingScreen>();
        AddChild(_loadingScreen);
      }

      LoadingLevel = true;
    }
  }

  public void OpenLevel(string levelName) {
    var levelpath = Game.Instance.LevelsDirectoryPath + "/" + levelName + ".tscn";
    var level = ResourceLoader.Load<PackedScene>(levelpath).Instantiate<Level>();
    OpenLevel(level);
  }

  public void ProcessThrededLevelLoad() {
    if (LoadingLevel) {
      GetLevelLoadStatus(out var status, out var progress);

      switch (status) {
        case ResourceLoader.ThreadLoadStatus.Loaded:
          var level = ((PackedScene)ResourceLoader.LoadThreadedGet(_loadedLevelPath)).Instantiate<Level>();
          _loadingScreen?.SetProgressBarValue(100);
          OpenLevel(level);
          LoadingLevel = false;
          break;
        case ResourceLoader.ThreadLoadStatus.InvalidResource:
          break;
        case ResourceLoader.ThreadLoadStatus.InProgress:
          _loadingScreen?.SetProgressBarValue(progress * 100);
          break;
        case ResourceLoader.ThreadLoadStatus.Failed:
          break;
        default:
          break;
      }
    }
  }

  private void GetLevelLoadStatus(out ResourceLoader.ThreadLoadStatus status, out float progress) {
    Array progressArr = new();
    status = ResourceLoader.LoadThreadedGetStatus(_loadedLevelPath, progressArr);
    progress = progressArr[0].As<float>();
  }


  private void OpenLevel(Level level) {
    if (level == null) {
      GD.PrintErr("Loaded level is null.");
      return;
    }

    if (CurrentLevel != null) {
      CurrentLevel.UnLoad();
      CurrentLevel.TreeExited += () => SwitchLevel(level);
    }
    else {
      SwitchLevel(level);
    }
  }

  private void SwitchLevel(Level level) {
    CurrentLevel = level;

    CurrentLevel.TreeEntered += () => {
      CurrentLevel.Load();
      EmitSignal(SignalName.LevelLoaded, level);
    };

    AddChild(CurrentLevel);
  }
}
