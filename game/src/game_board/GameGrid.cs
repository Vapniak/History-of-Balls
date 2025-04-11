namespace HOB;

using System;
using System.Collections.Generic;
using Godot;
using HexGridMap;


[Tool]
public class GameGrid : HexGrid<GameCell, GameGridLayout> {
  public MapData MapData { get; private set; }
  public GameGrid(GameGridLayout layout) : base(layout) {
  }

  public (Vector3 first, Vector3 second) GetCorners(HexDirection direction) {
    var first = GetLayout().GetCorner((int)direction);
    var second = GetLayout().GetCorner((int)direction.Next());
    return (new(first.X, 0, first.Y), new(second.X, 0, second.Y));
  }

  public (Vector3 first, Vector3 second) GetSolidCorners(HexDirection direction) {
    var (first, second) = GetCorners(direction);
    return (first * GetLayout().SolidFactor, second * GetLayout().SolidFactor);
  }

  public (Vector3 first, Vector3 second) GetWaterCorners(HexDirection direction) {
    var (first, second) = GetCorners(direction);
    return (first * GetLayout().WaterFactor, second * GetLayout().WaterFactor);
  }

  public Vector3 GetBridge(HexDirection direction) {
    var (first, second) = GetCorners(direction);
    return (first + second) * GetLayout().BlendFactor;
  }

  public Vector3 GetWaterBridge(HexDirection direction) {
    var (first, second) = GetCorners(direction);
    return (first + second) * GetLayout().WaterBlendFactor;
  }


  public Vector3 GetCellRealPosition(GameCell cell) {
    return new(cell.Position.X, GetSetting(cell).Elevation * GetLayout().ElevationStep, cell.Position.Y);
  }


  public GameCell.EdgeType GetEdgeType(GameCell from, GameCell to) {
    var fromElevation = GetSetting(from).Elevation;
    var toEleveation = GetSetting(to).Elevation;

    var delta = Mathf.Abs(fromElevation - toEleveation);

    if (delta <= GetLayout().FlatMaxElevationDelta) {
      return GameCell.EdgeType.Flat;
    }

    if (delta <= GetLayout().SlopeMaxElevationDelta) {
      return GameCell.EdgeType.Slope;
    }

    return GameCell.EdgeType.Cliff;
  }


  public GameCell[] ExpandSearch(GameCell start, uint maxCost, Func<GameCell, GameCell, bool> isReachable) {
    var minCost = new int[GetCells().Length];
    for (var i = 0; i < minCost.Length; i++) {
      minCost[i] = int.MaxValue;
    }

    minCost[GetCellIndex(start)] = 0;

    var reachableCells = new List<GameCell>();
    var pq = new PriorityQueue<GameCell, int>();
    pq.Enqueue(start, 0);

    while (pq.Count > 0) {
      var current = pq.Dequeue();
      var currentCost = minCost[GetCellIndex(current)];

      if (currentCost > maxCost) {
        continue;
      }

      reachableCells.Add(current);

      for (var i = HexDirection.First; i <= HexDirection.Sixth; i++) {
        var cell = GetCell(current, i);
        if (cell != null && isReachable(current, cell)) {
          var newCost = currentCost + GetSetting(cell).MoveCost;
          var cellIndex = GetCellIndex(cell);
          if (newCost < minCost[cellIndex]) {
            minCost[cellIndex] = newCost;
            pq.Enqueue(cell, newCost);
          }
        }
      }
    }

    reachableCells.RemoveAt(0);

    return reachableCells.ToArray();
  }


  public GameCell[] FindPath(GameCell start, GameCell target, uint maxCost, Func<GameCell, GameCell, bool> isReachable) {
    var minCost = new int[GetCells().Length];
    var parent = new GameCell[GetCells().Length];
    var visited = new bool[GetCells().Length];

    for (var i = 0; i < minCost.Length; i++) {
      minCost[i] = int.MaxValue;
      parent[i] = null;
      visited[i] = false;
    }

    minCost[GetCellIndex(start)] = 0;
    var pq = new PriorityQueue<GameCell, int>();
    pq.Enqueue(start, 0);

    var closestCell = start;
    var closestDistance = start.Coord.Distance(target.Coord);

    while (pq.Count > 0) {
      var current = pq.Dequeue();
      var currentCost = minCost[GetCellIndex(current)];

      if (current == target) {
        return ReconstructPath(parent, current, maxCost);
      }

      var currentDistance = current.Coord.Distance(target.Coord);
      if (currentDistance < closestDistance) {
        closestCell = current;
        closestDistance = currentDistance;
      }

      visited[GetCellIndex(current)] = true;

      for (var i = (int)HexDirection.First; i <= (int)HexDirection.Sixth; i++) {
        var cell = GetCell(current, (HexDirection)i);
        if (cell != null && isReachable(current, cell) && !visited[GetCellIndex(cell)]) {
          var newCost = currentCost + GetSetting(cell).MoveCost;
          var cellIndex = GetCellIndex(cell);
          if (newCost < minCost[cellIndex]) {
            minCost[cellIndex] = newCost;
            parent[cellIndex] = current;
            pq.Enqueue(cell, newCost);
          }
        }
      }
    }

    return ReconstructPath(parent, closestCell, maxCost);
  }

  private GameCell[] ReconstructPath(GameCell[] parent, GameCell endCell, uint maxCost) {
    var path = new List<GameCell>();
    var currentCell = endCell;
    while (currentCell != null) {
      path.Add(currentCell);
      currentCell = parent[GetCellIndex(currentCell)];
    }

    path.Reverse();
    path.RemoveAt(0);

    var finalPath = new List<GameCell>();
    var totalCost = 0;
    foreach (var cell in path) {
      totalCost += GetSetting(cell).MoveCost;
      if (totalCost <= maxCost) {
        finalPath.Add(cell);
      }
      else {
        break;
      }
    }

    return finalPath.ToArray();
  }

  public CellSetting GetSetting(GameCell cell) {
    return MapData.Settings.CellSettings[cell.SettingId];
  }

  public Vector3 TerraceLerp(Vector3 a, Vector3 b, int step) {
    var h = step * GetLayout().HorizontalTerraceStepSize;
    a.X += (b.X - a.X) * h;
    a.Z += (b.Z - a.Z) * h;
    var v = (step + 1) / 2f * GetLayout().VerticalTerraceStepSize;
    a.Y += (b.Y - a.Y) * v;
    return a;
  }

  public Chunk.EdgeVertices TerraceLerp(Chunk.EdgeVertices a, Chunk.EdgeVertices b, int step) {
    Chunk.EdgeVertices result;
    result.V1 = TerraceLerp(a.V1, b.V1, step);
    result.V2 = TerraceLerp(a.V2, b.V2, step);
    result.V3 = TerraceLerp(a.V3, b.V3, step);
    result.V4 = TerraceLerp(a.V4, b.V4, step);
    result.V5 = TerraceLerp(a.V5, b.V5, step);
    return result;
  }

  public Vector2I GetMapSize() {
    return new(MapData.Cols, MapData.Rows);
  }

  public Vector2 GetRealMapSize() {
    return GetMapSize() * GetLayout().GetSpacingBetweenHexes();
  }
  public void LoadMap(MapData mapData) {
    MapData = mapData;
    var c = MapData.GetCells();

    var cells = new List<GameCell>();
    for (var x = 0; x < MapData.Cols; x++) {
      for (var y = 0; y < MapData.Rows; y++) {
        var hex = MapData.GetCell(x, y);
        var cell = new GameCell(
            GetLayout().OffsetToCube(new OffsetCoord(x, y)),
            GetLayout(),
            hex.Id,
            this
        );
        cells.Add(cell);
      }
    }

    CreateCells(cells.ToArray());
  }

  public IEnumerable<GameCell> GetEdgeCells() {
    foreach (var cell in GetCells()) {
      if (IsEdgeCell(cell.OffsetCoord)) {
        yield return cell;
      }
    }
  }

  private bool IsEdgeCell(OffsetCoord coord) {
    return coord.Col == 0 ||
           coord.Col == MapData.Cols - 1 ||
           coord.Row == 0 ||
           coord.Row == MapData.Rows - 1;
  }
}
