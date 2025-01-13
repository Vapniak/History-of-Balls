namespace HexGridMap;

using Godot;

public struct HexCell {
  public HexCoordinates Coordinates { get; private set; }

  private readonly HexLayout Layout { get; }
  public HexCell(HexCoordinates coordinates, HexLayout layout) {
    Coordinates = coordinates;
    Layout = layout;
  }

  public HexCell(HexOffsetCoordinates coordinates, HexLayout layout) : this(layout.OffsetToHex(coordinates), layout) {

  }

  public readonly Vector2 GetPoint() {
    return Layout.HexToPoint(Coordinates);
  }

  public readonly HexOffsetCoordinates GetOffsetCoordinates() {
    return Layout.HexToOffset(Coordinates);
  }
}
