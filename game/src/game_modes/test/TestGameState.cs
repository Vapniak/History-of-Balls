namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;

[GlobalClass]
public partial class TestGameState : GameState {
  public HexGrid HexGrid { get; set; }
}
