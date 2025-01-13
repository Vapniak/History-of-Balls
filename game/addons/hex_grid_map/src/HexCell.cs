namespace HexGridMap;

using Godot;

public class HexCell {
  public HexCoordinates Coordinates { get; private set; }

  private HexLayout Layout { get; }
  public HexCell(HexCoordinates coordinates, HexLayout layout) {
    Coordinates = coordinates;
    Layout = layout;
  }

  public HexCell(HexOffsetCoordinates coordinates, HexLayout layout) : this(layout.OffsetToHex(coordinates), layout) {

  }

  public Vector2 GetPoint() {
    return Layout.HexToPoint(Coordinates);
  }

  public HexOffsetCoordinates GetOffsetCoordinates() {
    return Layout.HexToOffset(Coordinates);
  }
}
