namespace GameplayFramework;

using Godot;
using Godot.Collections;

/// <summary>
/// Base class for all worlds.
/// </summary>
[GlobalClass]
public partial class World : Node {
  [Export] private GameMode GameMode { get; set; }

  public Level CurrentLevel { get; private set; }

  public override void _EnterTree() {
    if (Game.Instance.World != null) {
      GD.PrintErr("World already exists.");
      return;
    }

    Game.Instance.World = this;

    foreach (var child in GetChildren()) {
      if (child is Level level) {
        CurrentLevel = level;
        break;
      }
    }
  }

  public override void _ExitTree() {
    Game.Instance.World = null;
  }

  public GameMode GetGameMode() => GameMode;
}
