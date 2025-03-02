namespace GameplayFramework;

using System.Diagnostics;
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

  public async Task OpenLevelThreaded(string levelName, PackedScene loadingScreenScene = null) {
    if (LoadingLevel) {
      GD.PrintErr("Already loading a level!");
      return;
    }

    _loadedLevelPath = GameInstance.Instance.LevelsDirectoryPath + "/" + levelName + ".tscn";
    var error = ResourceLoader.LoadThreadedRequest(_loadedLevelPath, useSubThreads: true);

    if (error != Error.Ok) {
      GD.PrintErr("Failed to start threaded load for level: ", levelName);
      return;
    }

    if (IsInstanceValid(CurrentLevel)) {
      await CurrentLevel.UnLoad();
    }

    if (loadingScreenScene != null) {
      _loadingScreen = loadingScreenScene.Instantiate<LoadingScreen>();
      AddChild(_loadingScreen);
    }

    LoadingLevel = true;
  }

  public async Task ProcessThrededLevelLoad() {
    GetLevelLoadStatus(out var status, out var progress);

    switch (status) {
      case ResourceLoader.ThreadLoadStatus.Loaded:
        var level = ((PackedScene)ResourceLoader.LoadThreadedGet(_loadedLevelPath)).Instantiate<Level>();

        LoadingLevel = false;

        if (_loadingScreen != null) {
          _loadingScreen.SetProgressBarValue(100);
          await ToSignal(_loadingScreen, LoadingScreen.SignalName.FullReached);
          await Task.Delay(100);
        }

        SwitchLevel(level);
        break;

      case ResourceLoader.ThreadLoadStatus.InProgress:
        _loadingScreen?.SetProgressBarValue(progress * 100);
        break;

      case ResourceLoader.ThreadLoadStatus.Failed:
        break;
      case ResourceLoader.ThreadLoadStatus.InvalidResource:
        break;
    }
  }

  private void GetLevelLoadStatus(out ResourceLoader.ThreadLoadStatus status, out float progress) {
    Array progressArr = new();
    status = ResourceLoader.LoadThreadedGetStatus(_loadedLevelPath, progressArr);
    progress = progressArr[0].As<float>();
  }

  public async Task OpenLevel(string levelName) {
    var levelPath = GameInstance.Instance.LevelsDirectoryPath + "/" + levelName + ".tscn";
    var level = ResourceLoader.Load<PackedScene>(levelPath).Instantiate<Level>();

    if (IsInstanceValid(CurrentLevel)) {
      await CurrentLevel.UnLoad();
    }

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
