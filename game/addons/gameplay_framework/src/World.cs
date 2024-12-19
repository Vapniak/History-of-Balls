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
      GD.PrintErr("World already exists.");
    }

    Game.Instance.World = this;

    foreach (var child in GetChildren()) {
      if (child is Level level) {
        _levels.Add(level);
      }
    }
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
