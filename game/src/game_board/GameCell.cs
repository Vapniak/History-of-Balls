namespace HOB;

using Godot;
using HexGridMap;

public class GameCell : HexCell {
  public Color HighlightColor = Colors.Transparent;
  public GameCell(CubeCoord coord, HexLayout layout) : base(coord, layout) {
  }
}
