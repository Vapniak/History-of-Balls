namespace HOB.Core;

using Godot;

/// <summary>
/// Base class for all worlds (levels).
/// </summary>
[GlobalClass]
public sealed partial class World : Node3D {
  [Export] public GameMode GameMode { get; private set; } = new();
  public override void _Ready() {
    SpawnPlayer();
  }

  private void SpawnPlayer() {
    // TODO: add spawn location
    var player = GameMode.PlayerScene.Instantiate();
    AddChild(player);
  }
}
