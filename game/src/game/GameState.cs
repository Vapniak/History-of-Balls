namespace HOB;

using Godot;
using HexGridMap;


public class GameState {
  public HexGrid HexGrid { get; private set; }

  public void SetHexGrid(HexGrid grid) {
    HexGrid = grid;
  }
}
