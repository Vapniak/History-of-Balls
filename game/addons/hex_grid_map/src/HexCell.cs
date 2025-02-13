namespace HexGridMap;

using Godot;

// TODO: add hex cell interface, and use it in hex grid generic type
public partial class HexCell : RefCounted {
  public CubeCoord Coord { get; private set; }
  public OffsetCoord OffsetCoord => Layout.CubeToOffset(Coord);
  public Vector2 Position => Layout.CubeToPoint(Coord);

  private HexLayout Layout { get; set; }
  public HexCell(CubeCoord coord, HexLayout layout) {
    Coord = coord;
    Layout = layout;
  }


  public Vector2 GetCorner(HexDirection index) {
    var angle = Mathf.DegToRad((60f * (int)index) - Mathf.RadToDeg(Layout.Orientation.StartAngle));
    return new Vector2(Position.X + (Layout.HexCellSize * (float)Mathf.Cos(angle)), Position.Y + (Layout.HexCellSize * (float)Mathf.Sin(angle)));
  }
}
