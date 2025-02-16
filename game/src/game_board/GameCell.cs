namespace HOB;

using System;
using Godot;
using HexGridMap;

public partial class GameCell : HexCell {
  public enum EdgeType {
    Flat, // ELEVATION DIFF 0
    Slope, // ELEVATION DIFF 1
    Cliff // ELEVATION DIFF > 1
  }

  public int Index => Grid.GetCellIndex(this);
  public int SettingId { get; private set; }
  private GameGrid Grid { get; set; }
  public GameCell(CubeCoord coord, HexLayout layout, int settingId, GameGrid grid) : base(coord, layout) {
    SettingId = settingId;
    Grid = grid;
  }

  public Vector3 GetRealPosition() {
    return Grid.GetCellRealPosition(this);
  }

  public CellSetting GetSetting() => Grid.GetSetting(this);

  public EdgeType GetEdgeTypeTo(GameCell cell) {
    return Grid.GetEdgeType(this, cell);
  }

  public GameCell[] FindPathTo(GameCell cell, uint maxCost, Func<GameCell, GameCell, bool> isReachable) {
    return Grid.FindPath(this, cell, maxCost, isReachable);
  }

  public GameCell[] ExpandSearch(uint maxCost, Func<GameCell, GameCell, bool> isReachable) {
    return Grid.ExpandSearch(this, maxCost, isReachable);
  }

  public GameCell[] GetCellsInRange(uint range) {
    return Grid.GetCellsInRange(Coord, range);
  }
}
