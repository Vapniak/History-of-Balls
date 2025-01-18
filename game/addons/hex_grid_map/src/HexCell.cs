namespace HexGridMap;

using Godot;

public class HexCell {
  public CubeCoord Coord { get; private set; }

  private HexGrid Grid { get; }
  public HexCell(CubeCoord coord, HexGrid grid) {
    Coord = coord;
    Grid = grid;
  }

  public HexCell(OffsetCoord coord, HexGrid grid) {
    Grid = grid;

    Coord = GetLayout().OffsetToHex(coord);
  }

  public Vector2 GetPoint() {
    return GetLayout().HexToPoint(Coord);
  }

  public OffsetCoord GetOffsetCoord() {
    return GetLayout().HexToOffset(Coord);
  }

  public HexCell[] GetCellsInRange(int range) {
    return Grid.GetCells(GetLayout().CoordsInRange(Coord, range));
  }

  public HexLayout GetLayout() => Grid.GetLayout();
}
