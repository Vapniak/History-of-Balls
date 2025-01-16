namespace HexGridMap;

using Godot;

public class HexCell {
  public CubeCoord Coord { get; private set; }

  private HexLayout Layout { get; }
  public HexCell(CubeCoord coord, HexLayout layout) {
    Coord = coord;
    Layout = layout;
  }

  public HexCell(OffsetCoord coord, HexLayout layout) : this(layout.OffsetToHex(coord), layout) {

  }

  public Vector2 GetPoint() {
    return Layout.HexToPoint(Coord);
  }

  public OffsetCoord GetOffsetCoord() {
    return Layout.HexToOffset(Coord);
  }
}
