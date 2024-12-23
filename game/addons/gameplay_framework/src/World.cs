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

  public void OpenLevel(Level level) {
    if (CurrentLevel != null) {
      CurrentLevel.UnLoad();
      CurrentLevel.QueueFree();
    }
    CurrentLevel = level;
    CurrentLevel.Load();
    AddChild(CurrentLevel);
  }
  public GameMode GetGameMode() => CurrentLevel.GameMode;
}
