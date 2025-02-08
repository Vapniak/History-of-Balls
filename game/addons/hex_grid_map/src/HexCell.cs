namespace HexGridMap;

using Godot;

// TODO: add hex cell interface, and use it in hex grid generic type

public class HexCell {
  public CubeCoord Coord { get; private set; }
  public OffsetCoord OffsetCoord => Layout.CubeToOffset(Coord);
  public Vector2 Position => Layout.CubeToPoint(Coord);

  private HexLayout Layout { get; set; }
  public HexCell(CubeCoord coord, HexLayout layout) {
    Coord = coord;
    Layout = layout;
  }
}
