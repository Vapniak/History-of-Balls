namespace HexGridMap;

using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class HexGrid : Node {
  [Export] private GridShape GridShape { get; set; }
  [Export] private HexLayout Layout { get; set; } = new();
  private readonly HashSet<HexCoordinates> _grid = new();

  public void CreateGrid() {
    _grid.Clear();
    GridShape.CreateGrid(this);
  }

  public void AddHex(HexCoordinates hex) {
    _grid.Add(hex);
  }

  public void RemoveHex(HexCoordinates hex) {
    _grid.Remove(hex);
  }

  public int CellCount() {
    return _grid.Count;
  }

  public HexCoordinates[] GetCells() {
    return _grid.ToArray();
  }

  public HexLayout GetLayout() => Layout;
}
