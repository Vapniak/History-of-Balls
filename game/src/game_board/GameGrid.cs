namespace HOB;

using System;
using HexGridMap;


public class GameGrid : HexGrid<GameCell> {
  public GameGrid(HexLayout layout, GridShape shape) : base(layout, shape) {
  }
}
