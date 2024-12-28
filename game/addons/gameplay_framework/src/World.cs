namespace GameplayFramework;

using Godot;

/// <summary>
/// Base class for all worlds.
/// </summary>
[GlobalClass]
public sealed partial class World : Node {
  public Level CurrentLevel { get; private set; }

  public override void _EnterTree() {
    if (Game.Instance.World != null) {
      GD.PrintErr("World already exists.");
      return;
    }

    Game.Instance.World = this;

    CurrentLevel = GetChild<Level>(0);
    CurrentLevel?.Load();
  }

  public override void _ExitTree() {
    Game.Instance.World = null;
  }

  private void OpenLevel(Level level) {
    if (level == null) {
      GD.PrintErr("Loaded level is null.");
      return;
    }
    if (CurrentLevel != null) {
      CurrentLevel.UnLoad();
      CurrentLevel.QueueFree();
    }
    CurrentLevel = level;
    CurrentLevel.Load();
    AddChild(CurrentLevel);
  }


  /// <summary>
  /// Loads level by its name.
  /// </summary>
  /// <param name="levelName">Name of saved level scene.</param>
  public void OpenLevel(string levelName) {
    var scene = ResourceLoader.Load<PackedScene>(Game.Instance.LevelsDirectoryPath + "/" + levelName + ".tscn");
    var level = scene.Instantiate<Level>();
    OpenLevel(level);
  }
  public GameMode GetGameMode() => CurrentLevel.GameMode;
}
