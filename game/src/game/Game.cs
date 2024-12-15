namespace HOB;

using Godot;
using System;

public partial class Game : Node3D {

  // TODO: provide this gamestate to other nodes
  public GameState GameState { get; set; } = default!;

  public override void _Ready() {
    GameState = new();
  }
}
