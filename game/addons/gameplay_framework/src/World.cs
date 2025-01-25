namespace GameplayFramework;

using System.Diagnostics;
using Godot;

/// <summary>
/// Base class for all worlds.
/// </summary>
[GlobalClass]
public sealed partial class World : Node {
  [Signal] public delegate void LevelOpenedEventHandler(Level level);

  public Level CurrentLevel { get; private set; }

  public GameMode GetGameMode() => CurrentLevel.GameMode;

  /// <summary>
  /// Loads level by its name.
  /// </summary>
  /// <param name="levelName">Name of saved level scene.</param>
  public void OpenLevel(string levelName) {
    var scene = ResourceLoader.Load<PackedScene>(Game.Instance.LevelsDirectoryPath + "/" + levelName + ".tscn");
    var level = scene.Instantiate<Level>();
    OpenLevel(level);
  }


  private void OpenLevel(Level level) {
    if (level == null) {
      GD.PrintErr("Loaded level is null.");
      return;
    }


    if (CurrentLevel != null) {
      CurrentLevel.UnLoad();
      CurrentLevel.TreeExited += () => SwitchLevel(level);
      CurrentLevel.QueueFree();
    }
    else {
      SwitchLevel(level);
    }
  }

  private void SwitchLevel(Level level) {
    EmitSignal(SignalName.LevelOpened, level);
    CurrentLevel = level;

    CurrentLevel.Loaded += () => AddChild(CurrentLevel);
    CurrentLevel.Load();
  }
}
