namespace HOB;

using Godot;
using HexGridMap;

public class GameCell : HexCell {
  public Color HighlightColor { get; set; } = Colors.Transparent;
  public int MoveCost { get; set; } = 1;
  public int Elevation { get; set; }
  public GameCell(CubeCoord coord, HexLayout layout) : base(coord, layout) {
  }

  public Vector3 GetRealPosition() {
    return new(Position.X, Elevation, Position.Y);
  }
}
