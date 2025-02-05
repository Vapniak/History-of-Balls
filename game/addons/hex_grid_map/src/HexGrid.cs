namespace HexGridMap;

using System;
using System.Collections.Generic;
using Godot;
using HOB;

public abstract partial class HexGrid<T> where T : HexCell {
  [Export] private GridShape GridShape { get; set; }
  [Export] private HexLayout Layout { get; set; }

  private T[] Cells { get; set; }

  private Dictionary<CubeCoord, int> CoordToIndexMap { get; set; }

  public HexGrid(HexLayout layout, GridShape shape) {
    Layout = layout;
    GridShape = shape;
  }

  public void CreateCells(Func<CubeCoord, T> createCell) {
    CoordToIndexMap = new();
    Cells = GridShape.CreateCells(createCell, Layout);

    for (var i = 0; i < Cells.Length; i++) {
      CoordToIndexMap.Add(Cells[i].Coord, i);
    }
  }

  public T GetCell(Vector3 point) {
    var hex = GetLayout().PointToHex(new(point.X, point.Z));
    return GetCell(hex);
  }
  public T GetCell(CubeCoord coord) {
    if (CoordToIndexMap.TryGetValue(coord, out var index)) {
      return Cells[index];
    }
    return null;
  }

  public int GetCellIndex(GameCell cell) {
    return Array.IndexOf(Cells, cell);
  }

  public T GetCell(T cell, HexDirection direction) {
    return GetCell(cell.Coord.GetNeighbor(direction));
  }
  public T[] GetCells() => Cells;
  public T[] GetCells(CubeCoord[] coords) {
    List<T> cells = new();
    foreach (var coord in coords) {
      var cell = GetCell(coord);
      // TODO: find better solution to get cells and check if they exist
      if (cell != null) {
        cells.Add(cell);
      }
    }

    return cells.ToArray();
  }

  public T[] GetCellsInRange(CubeCoord center, uint range) {
    return GetCells(GetLayout().CoordsInRange(center, range));
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
