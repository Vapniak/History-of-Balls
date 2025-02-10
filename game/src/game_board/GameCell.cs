namespace HOB;

using Godot;
using HexGridMap;

public partial class GameCell : HexCell {
  public CellSettings Settings { get; private set; }
  public GameCell(CubeCoord coord, HexLayout layout, CellSettings settings) : base(coord, layout) {
    Settings = settings;
  }

  public Vector3 GetRealPosition() {
    return new(Position.X, Settings.Elevation, Position.Y);
  }
}
