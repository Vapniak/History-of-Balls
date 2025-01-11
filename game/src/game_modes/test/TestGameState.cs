namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TestGameState : GameState, IPlayerManagmentGameState, IPauseGameState, IMatchGameState {
  public GameBoard GameBoard { get; set; }

  public Array<PlayerState> PlayerArray { get; set; }
  public bool PauseGame { get; set; } = true;
}
