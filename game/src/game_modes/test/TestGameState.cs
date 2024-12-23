namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;
using HexGridMap;

[GlobalClass]
public partial class TestGameState : GameState, IPlayerManagmentGameState {
  public HexGrid HexGrid { get; set; }
  public Array<PlayerState> PlayerArray { get; set; }
}
