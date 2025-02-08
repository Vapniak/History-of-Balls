namespace GameplayFramework;

using Godot;
using Godot.Collections;

/// <summary>
/// Keeps track of overall state of current game etc. phase, game events, current time.
/// </summary>
[GlobalClass]
public partial class GameState : Resource, IGameState {
  public GameState() { }

  public virtual void Init() { }
}
