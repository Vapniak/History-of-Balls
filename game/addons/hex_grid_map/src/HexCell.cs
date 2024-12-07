namespace HexGridMap;

public struct HexCell {
  private readonly int _index;
  private HexGrid _grid;

  public HexCell(int index, HexGrid grid) {
    _index = index;
    _grid = grid;
  }
}
