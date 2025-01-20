namespace HexGridMap;

using Godot;

public class HexCell {
  public CubeCoord Coord { get; private set; }
  private HexLayout Layout { get; set; }
  public HexCell(CubeCoord coord, HexLayout layout) {
    Coord = coord;
    Layout = layout;
  }

  public Vector2 GetPoint() {
    return GetLayout().HexToPoint(Coord);
  }

  public OffsetCoord GetOffsetCoord() {
    return GetLayout().HexToOffset(Coord);
  }

  public HexLayout GetLayout() => Layout;
}
