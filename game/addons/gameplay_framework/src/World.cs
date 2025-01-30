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

  private bool LoadLevel { get; set; }

  private string _loadedLevelPath;
  private Node _loadingScreen;

  public GameMode GetGameMode() => CurrentLevel.GameMode;

  /// <summary>
  /// Loads level by its name.
  /// </summary>
  /// <param name="levelName">Name of saved level scene.</param>
  public void OpenLevel(string levelName, PackedScene loadingScreenScene = null) {
    _loadedLevelPath = Game.Instance.LevelsDirectoryPath + "/" + levelName + ".tscn";
    var scene = ResourceLoader.LoadThreadedRequest(_loadedLevelPath, useSubThreads: true);
    LoadLevel = true;

    if (loadingScreenScene != null) {
      _loadingScreen = loadingScreenScene.Instantiate();
      AddChild(_loadingScreen);
    }
  }

  public override void _Process(double delta) {
    if (LoadLevel) {
      GetLevelLoadStatus(out var status, out _);

      switch (status) {
        case ResourceLoader.ThreadLoadStatus.Loaded:
          var level = ((PackedScene)ResourceLoader.LoadThreadedGet(_loadedLevelPath)).Instantiate<Level>();
          OpenLevel(level);
          // FIXME: some flickering of scene when loading
          if (IsInstanceValid(_loadingScreen)) {
            _loadingScreen.QueueFree();
          }
          LoadLevel = false;
          break;
        case ResourceLoader.ThreadLoadStatus.InvalidResource:
          break;
        case ResourceLoader.ThreadLoadStatus.InProgress:
          break;
        case ResourceLoader.ThreadLoadStatus.Failed:
          break;
        default:
          break;
      }
    }
  }

  public void GetLevelLoadStatus(out ResourceLoader.ThreadLoadStatus status, out float progress) {
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
      CurrentLevel.Free();
    }
    else {
      SwitchLevel(level);
    }
  }

  private void SwitchLevel(Level level) {
    CurrentLevel = level;

    CurrentLevel.Load();
    AddChild(CurrentLevel);

    EmitSignal(SignalName.LevelLoaded, level);
  }
}
