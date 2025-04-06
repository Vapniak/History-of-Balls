namespace GameplayFramework;

using Godot;

/// <summary>
/// Keeps track of overall state of current game etc. phase, current time.
/// </summary>
[GlobalClass]
public partial class GameState : Resource, IGameState {
  public ulong GameTimeMSec { get; set; }

  public GameState() { }
}
