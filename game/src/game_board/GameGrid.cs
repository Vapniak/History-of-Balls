namespace HOB;

using HexGridMap;


public class GameGrid : HexGrid<GameCell> {
  public GameGrid(HexLayout layout) : base(layout) {
  }
}
