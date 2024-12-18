namespace GameplayFramework;

using Godot;
using Godot.Collections;

/// <summary>
/// Keeps track of all variables and state of current game.
/// </summary>
[GlobalClass]
public partial class GameState : Resource {
  public Array<PlayerState> PlayerArray = new();
  public GameState() { }
}
