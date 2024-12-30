namespace HexGridMap;

using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class HexGrid : Node {
  [Export] private GridShape GridShape { get; set; }
  [Export] private HexLayout Layout { get; set; } = new();
  private readonly HashSet<HexCell> _grid = new();

  public void CreateGrid() {
    _grid.Clear();
    GridShape.CreateGrid(this);
  }

  public void AddHex(HexCell hex) {
    _grid.Add(hex);
  }

  public int CellCount() {
    return _grid.Count;
  }

  public void RemoveHex(HexCell hex) {
    _grid.Remove(hex);
  }

  public HexCell[] GetCells() {
    return _grid.ToArray();
  }

  public HexLayout GetLayout() => Layout;
}
