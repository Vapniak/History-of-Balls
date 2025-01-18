namespace HexGridMap;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class HexGrid : Node {
  [Export] private GridShape GridShape { get; set; }
  [Export] private HexLayout Layout { get; set; }

  private HexCell[] Cells { get; set; }

  private Dictionary<CubeCoord, int> CoordToIndexMap { get; set; }
  public void CreateGrid() {
    CoordToIndexMap = new();
    Cells = GridShape.CreateCells(this);
    for (var i = 0; i < Cells.Length; i++) {
      CoordToIndexMap.Add(Cells[i].Coord, i);
    }
  }

  public HexCell GetCell(Vector3 point) {
    var hex = GetLayout().PointToHex(new(point.X, point.Z));
    return GetCell(hex);
  }
  public HexCell GetCell(CubeCoord coord) {
    if (CoordToIndexMap.TryGetValue(coord, out var index)) {
      return Cells[index];
    }
    return null;
  }

  public HexCell[] GetCells() => Cells;
  public HexCell[] GetCells(CubeCoord[] coords) {
    List<HexCell> cells = new();
    foreach (var coord in coords) {
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
