namespace HexGridMap;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class HexGrid : Node {
  [Export] private GridShape GridShape { get; set; }
  [Export] private HexLayout Layout { get; set; }

  public HexCell[] Cells { get; private set; }

  private Dictionary<HexCoordinates, int> CoordToIndexMap { get; set; }
  public void CreateGrid() {
    CoordToIndexMap = new();
    Cells = GridShape.CreateCells(GetLayout());
    for (var i = 0; i < Cells.Length; i++) {
      CoordToIndexMap.Add(Cells[i].Coordinates, i);
    }
  }

  public HexCell GetCell(HexCoordinates coordinates) {
    return Cells[CoordToIndexMap[coordinates]];
  }

  public HexCell[] GetCells(HexCoordinates[] coordinates) {
    List<HexCell> cells = new();
    foreach (var coord in coordinates) {
      cells.Add(GetCell(coord));
    }

    return cells.ToArray();
  }

  public HexLayout GetLayout() => Layout;
  public GridShape GetGridShape() => GridShape;

  public Vector2I GetRectSize() {
    return GetGridShape().GetRectSize();
  }

  public Vector2 GetRealSize() {
    var size = new Vector2();

    var m = GetLayout().Orientation;
    // FIXME: fix the size, some misaligment is visible
    size.X = GetRectSize().X * GetLayout().HexCellScale * m.F0;
    size.Y = GetRectSize().Y * GetLayout().HexCellScale * m.F3;

    return size;
  }
}
