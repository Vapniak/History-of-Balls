namespace HOB;

using Godot;
using HexGridMap;

public class GameCell : HexCell {
  public Color HighlightColor { get; set; } = Colors.Transparent;
  public Color TerrainColor { get; set; } = Colors.White;
  public int MoveCost { get; init; } = 1;
  public int Elevation { get; init; }
  public GameCell(CubeCoord coord, HexLayout layout) : base(coord, layout) {
  }

  public Vector3 GetRealPosition() {
    return new(Position.X, Elevation, Position.Y);
  }
}
