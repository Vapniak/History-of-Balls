namespace HexGridMap;


public struct HexCell {
  public HexCoordinates Coordinates { get; private set; }

  public HexCell(HexCoordinates coordinates) {
    Coordinates = coordinates;
  }
}
