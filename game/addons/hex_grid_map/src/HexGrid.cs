namespace HexGridMap;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class HexGrid : Node {
  [Export] public HexLayout Layout { get; private set; } = new();
  [Export] public GridShape GridShape { get; private set; }
  private readonly HashSet<HexCell> _grid = new();

  public HexGrid() {

  }
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

  public HashSet<HexCell>.Enumerator GetGridEnumerator() {
    return _grid.GetEnumerator();
  }
}
