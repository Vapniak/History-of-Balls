namespace GameplayFramework;

using Godot;

/// <summary>
/// Base class for all worlds.
/// </summary>
[GlobalClass]
public sealed partial class World : Node {
  [Signal] public delegate void LevelLoadedEventHandler(Level level);

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
    CurrentLevel = level;

    CurrentLevel.Load();
    AddChild(CurrentLevel);

    EmitSignal(SignalName.LevelLoaded, level);
  }
}
