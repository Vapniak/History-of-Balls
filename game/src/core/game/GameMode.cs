namespace HOB.Core;

using Godot;

/// <summary>
/// Overall game manager.
/// </summary>
[GlobalClass]
public partial class GameMode : Resource {
  [Export] public PackedScene PlayerScene { get; private set; } = new();
  [Export] public GameState GameState { get; private set; } = new();
  [Export] public PlayerState PlayerState { get; private set; } = new();
  public GameMode() { }
}
