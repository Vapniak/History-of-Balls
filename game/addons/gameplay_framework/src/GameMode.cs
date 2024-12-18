namespace GameplayFramework;

using Godot;
using System;

/// <summary>
/// Manages game logic.
/// </summary>
[GlobalClass]
public partial class GameMode : Node {
  [Export] internal PackedScene PlayerScene { get; set; }
  [Export] internal GameState GameState { get; set; }
  [Export] internal PlayerState PlayerState { get; set; }

  public override void _Ready() {
    GameState = new();

    CallDeferred(nameof(SpawnPlayer));
  }

  public virtual void SpawnPlayer() {
    var player = PlayerScene?.Instantiate();
    if (player != null) {
      PlayerState = new();
      GameState.PlayerArray.Add(PlayerState);
      Game.GetWorld().AddChild(player);
    }
  }
}
