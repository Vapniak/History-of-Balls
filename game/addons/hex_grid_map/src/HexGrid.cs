namespace HexGridMap;

using System;
using System.Collections.Generic;
using Godot;
using HOB;

public abstract partial class HexGrid<TCell, TLayout>
where TCell : HexCell
where TLayout : HexLayout {
  private TLayout Layout { get; set; }

  private TCell[] Cells { get; set; }

  private Dictionary<CubeCoord, int> CoordToIndexMap { get; set; }

  public HexGrid(TLayout layout) {
    Layout = layout;
  }

  public void CreateCells(Func<CubeCoord, TCell> createCell, GridShape gridShape) {
    Cells = gridShape.CreateCells(createCell, Layout);
    Init();
  }

  public void CreateCells(TCell[] cells) {
    Cells = cells;
    Init();
  }

  private void Init() {
    CoordToIndexMap = new();
    for (var i = 0; i < Cells.Length; i++) {
      CoordToIndexMap.Add(Cells[i].Coord, i);
    }
  }

  public TCell GetCell(Vector3 point) {
    var hex = GetLayout().PointToCube(new(point.X, point.Z));
    return GetCell(hex);
  }

  public TCell GetCell(int index) {
    return Cells[index];
  }
  public TCell GetCell(CubeCoord coord) {
    if (CoordToIndexMap.TryGetValue(coord, out var index)) {
      return Cells[index];
    }
    return null;
  }

  public TCell[] GetNeighbors(TCell cell) {
    var cells = new List<TCell>();
    for (var dir = HexDirection.First; dir <= HexDirection.Sixth; dir++) {
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

  public TCell[] GetCellsInLine(TCell from, TCell to) {
    return GetCells(GetLayout().CoordsInLine(from.Coord, to.Coord));
  }

  public TCell[] GetCellsInRing(TCell center, uint radius) {
    var cells = new List<TCell>();
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
  private TCell? FindClosestValidCell(CubeCoord coord) {
    TCell? closestCell = null;
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

  public TCell GetCell(TCell cell, HexDirection direction) {
    return GetCell(cell.Coord.GetNeighbor(direction));
  }
  public TCell[] GetCells() => Cells;
  public TCell[] GetCells(CubeCoord[] coords) {
    List<TCell> cells = new();
    foreach (var coord in coords) {
      var cell = GetCell(coord);
      // TODO: find better solution to get cells and check if they exist
      if (cell != null) {
        cells.Add(cell);
      }
    }

    return cells.ToArray();
  }

  public TCell[] GetCellsInRange(CubeCoord center, uint range) {
    return GetCells(GetLayout().CoordsInRange(center, range));
  }

  public TLayout GetLayout() => Layout;
}
