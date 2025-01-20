namespace HOB;


using HexGridMap;


public class GameGrid : HexGrid<GameCell> {
  public GameGrid(HexLayout layout, GridShape shape) : base(layout, shape) {
  }

  public override GameCell CreateCell(CubeCoord coord) => new(coord, GetLayout());
}
