namespace HOB;

using Godot;
using HexGridMap;

public partial class GameCell : HexCell {
  public enum EdgeType {
    Flat, // ELEVATION DIFF 0
    Slope, // ELEVATION DIFF 1
    Hill // ELEVATION DIFF > 1
  }

  public int SettingId { get; private set; }
  public GameCell(CubeCoord coord, HexLayout layout, int settingId) : base(coord, layout) {
    SettingId = settingId;
  }
}
