namespace GameplayFramework;

using Godot;
using Godot.Collections;

/// <summary>
/// Base class for all worlds.
/// </summary>
[GlobalClass]
public partial class World : Node {
  [Export] internal GameMode GameMode { get; private set; }

  protected Array<Level> _levels = new();

  public override void _EnterTree() {
    if (Game.Instance.World != null) {
      GD.PrintErr("Last Level is not deleted before next one is added.");
    }

    Game.Instance.World = this;
  }

  public override void _ExitTree() {
    Game.Instance.World = null;
  }

  public void AddLevel(Level level) {
    AddChild(level);
    _levels.Add(level);
  }
  public void RemoveLevel(Level level) {
    RemoveChild(level);
    _levels.Remove(level);
  }
  public void ClearLevels() {
    foreach (var level in _levels) {
      _levels.Remove(level);
      level.QueueFree();
    }
  }
}
