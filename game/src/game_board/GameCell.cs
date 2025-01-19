namespace HOB;

using HexGridMap;

public class GameCell : HexCell {
  public GameCell(CubeCoord coord, GameGrid grid) : base(coord, grid) { }

  public GameCell(OffsetCoord coord, GameGrid grid) : base(coord, grid) { }
}
