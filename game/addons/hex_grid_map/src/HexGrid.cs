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
    if (CoordToIndexMap.TryGetValue(coordinates, out var index)) {
      return Cells[index];
    }
    return null;
  }


  public HexCell[] GetCells(HexCoordinates[] coordinates) {
    List<HexCell> cells = new();
    foreach (var coord in coordinates) {
      var cell = GetCell(coord);
      // TODO: find better solution to get cells and check if they exist
      if (cell != null) {
        cells.Add(cell);
      }
    }

    return cells.ToArray();
  }

  public HexLayout GetLayout() => Layout;
  public GridShape GetGridShape() => GridShape;

  public Vector2I GetRectSize() {
    return GetGridShape().GetRectSize();
  }

  public Vector2 GetRealSize() {

    // TODO: that's good for now
    return (Vector2)GetRectSize() * GetLayout().GetSpacingBetweenHexes();
  }
}
