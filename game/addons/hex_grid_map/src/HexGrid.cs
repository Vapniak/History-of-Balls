namespace HexGridMap;

using System;
using System.Collections.Generic;
using Godot;
using HOB;

public abstract partial class HexGrid<T> where T : HexCell {
  private HexLayout Layout { get; set; }

  private T[] Cells { get; set; }

  private Dictionary<CubeCoord, int> CoordToIndexMap { get; set; }

  public HexGrid(HexLayout layout) {
    Layout = layout;
  }

  public void CreateCells(Func<CubeCoord, T> createCell, GridShape gridShape) {
    Cells = gridShape.CreateCells(createCell, Layout);
    Init();
  }

  public void CreateCells(T[] cells) {
    Cells = cells;
    Init();
  }

  private void Init() {
    CoordToIndexMap = new();
    for (var i = 0; i < Cells.Length; i++) {
      CoordToIndexMap.Add(Cells[i].Coord, i);
    }
  }

  public T GetCell(Vector3 point) {
    var hex = GetLayout().PointToCube(new(point.X, point.Z));
    return GetCell(hex);
  }

  public T GetCell(int index) {
    return Cells[index];
  }
  public T GetCell(CubeCoord coord) {
    if (CoordToIndexMap.TryGetValue(coord, out var index)) {
      return Cells[index];
    }
    return null;
  }

  public T[] GetNeighbors(T cell) {
    var cells = new List<T>();
    for (var dir = HexDirection.Min; dir < HexDirection.Max; dir++) {
      var current = GetCell(cell, dir);
      if (current != null) {
        cells.Add(GetCell(cell, dir));
      }
    }

    return cells.ToArray();
  }

  public int GetCellIndex(GameCell cell) {
    return Array.IndexOf(Cells, cell);
  }

  public T[] GetCellsInLine(T from, T to) {
    return GetCells(GetLayout().CoordsInLine(from.Coord, to.Coord));
  }

  public T[] GetCellsInRing(T center, uint radius) {
    var cells = new List<T>();
    foreach (var coord in GetLayout().CoordsInRing(center.Coord, radius)) {
      var cell = GetCell(coord);
      if (cell == null) {
        cells.Add(FindClosestValidCell(coord));
      }
      else {
        cells.Add(cell);
      }
    }

    return cells.ToArray();
  }


  // FIXME: it gets the cell closest to coord not to edge of map which is wrong sometimes but works for
  private T FindClosestValidCell(CubeCoord coord) {
    T closestCell = null;
    var minDistance = int.MaxValue;

    foreach (var cell in GetCells()) {
      var distance = coord.Distance(cell.Coord);
      if (distance < minDistance) {
        minDistance = distance;
        closestCell = cell;
      }
    }

    return closestCell;
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
}
