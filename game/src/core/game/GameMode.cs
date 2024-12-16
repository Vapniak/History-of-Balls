namespace HOB.Core;

using Godot;
using System;

/// <summary>
/// Manages game logic.
/// </summary>
[GlobalClass]
public partial class GameMode : Node {
  [Export] internal PackedScene PlayerScene { get; set; }
  [Export] internal GameState GameState { get; set; } = new();
  [Export] internal PlayerState PlayerState { get; set; } = new();
}
