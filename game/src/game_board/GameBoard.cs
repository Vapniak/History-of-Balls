namespace HOB;

using System;
using System.Collections.Generic;
using Godot;
using HexGridMap;
using HOB.GameEntity;
using RaycastSystem;

/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();
  [Export] private HexLayout Layout { get; set; }
  [Export] private TerrainManager TerrainManager { get; set; }
  [Export] private EntityManager EntityManager { get; set; }
  [Export] public MapData MapData { get; private set; }

  private GameGrid Grid { get; set; }

  public void Init() {
    Grid = new(Layout);

    EntityManager.EntityRemoved += (entity) => {
      SetHighlight(entity.Cell, Colors.Transparent);
      UpdateHighlights();
    };

    LoadMap(MapData);

    TerrainManager.CreateData(MapData, GetRealMapSize());


    EmitSignal(SignalName.GridCreated);
  }

  public override void _Ready() {
    // TODO: add console and command for showing debug
    // #if DEBUG
    //     DebugDraw3D.DrawAabb(GetAabb(), Colors.Red, float.MaxValue);

    //     foreach (var cell in GetCells()) {
    //       DebugDraw3D.DrawSphere(new(cell.Position.X, 0, cell.Position.Y), 0.5f, Colors.Blue, float.MaxValue);
    //     }
    // #endif
  }

  public override void _PhysicsProcess(double delta) {
    if (Engine.IsEditorHint()) {
      return;
    }

  }

  public Aabb GetAabb() {
    var aabb = new Aabb {
      Size = new(GetRealMapSize().X, 1, GetRealMapSize().Y),
      // TODO: that offset probably will be wrong with different hex oreintations
      Position = new(-Grid.GetLayout().GetRealHexSize().X / 2, 0, -Grid.GetLayout().GetRealHexSize().Y / 2),
    };
    return aabb;
  }

  public void SetMouseHighlight(bool value) {
    TerrainManager.SetMouseHighlight(value);
  }


  // TODO: somehow only encapsulate this methods from grid
  public CubeCoord PointToCube(Vector3 point) {
    return Grid.GetLayout().PointToCube(new(point.X, point.Z));
  }

  public GameCell GetCell(CubeCoord coord) {
    return Grid.GetCell(coord);
  }
  public GameCell GetCell(Vector3 point) {
    return Grid.GetCell(point);
  }

  public GameCell[] GetCells() {
    return Grid.GetCells();
  }

  public GameCell GetCell(GameCell cell, HexDirection direction) {
    return Grid.GetCell(cell, direction);
  }

  public GameCell[] GetCells(CubeCoord[] coords) {
    return Grid.GetCells(coords);
  }

  public GameCell[] GetCellsInRange(CubeCoord center, uint range) {
    return Grid.GetCellsInRange(center, range);
  }

  public GameCell[] GetCellsInLine(GameCell from, GameCell to) {
    return Grid.GetCellsInLine(from, to);
  }

  public GameCell[] GetNeighbors(GameCell cell) {
    return Grid.GetNeighbors(cell);
  }

  public Vector2I GetMapSize() {
    return new(MapData.Cols, MapData.Rows);
  }

  public Vector2 GetRealMapSize() {
    return GetMapSize() * Layout.GetSpacingBetweenHexes();
  }

  public bool TryAddEntity(EntityData data, CubeCoord coord, IMatchController controller) {
    GameCell closestCell = null;
    var minDistance = int.MaxValue;

    foreach (var cell in GetCells()) {
      var distance = coord.Distance(cell.Coord);
      if (distance < minDistance && GetEntitiesOnCell(cell).Length == 0 && GetSetting(cell).MoveCost > 0) {
        minDistance = distance;
        closestCell = cell;
      }
    }

    var entity = Entity.Instantiate(controller, data, closestCell, this);
    EntityManager.AddEntity(entity);
    return true;
  }

  public Entity[] GetOwnedEntitiesOnCell(IMatchController owner, GameCell cell) {
    return EntityManager.GetOwnedEntitiesOnCell(owner, cell);
  }

  public Entity[] GetEntitiesOnCell(GameCell cell) {
    return EntityManager.GetEntitiesOnCell(cell);
  }

  public Entity[] GetOwnedEntities(IMatchController owner) {
    return EntityManager.GetOwnedEntites(owner);
  }

  public void SetHighlight(GameCell cell, Color color) {
    TerrainManager.SetHighlight(cell, color);
  }

  public void UpdateHighlights() {
    TerrainManager.UpdateHighlights();
  }

  public void ClearHighlights() {
    TerrainManager.ClearHighlights();
  }

  public Vector3 GetCellRealPosition(GameCell cell) {
    return new(cell.Position.X, GetSetting(cell).Elevation, cell.Position.Y);
  }

  // TODO: proper line of sight, for now it can be like this...
  public GameCell[] GetCellsInSight(GameCell center, uint range) {
    var visibleHexes = new List<GameCell>();

    foreach (var cell in Grid.GetCellsInRing(center, range)) {
      foreach (var c in GetCellsInLine(center, cell)) {
        if (c == center) {
          continue;
        }

        visibleHexes.Add(c);

        if (GetEntitiesOnCell(c).Length != 0) {
          break;
        }
      }
    }

    return visibleHexes.ToArray();
  }

  public GameCell[] FindReachableCells(GameCell start, uint maxCost, Func<GameCell, GameCell, bool> isReachable) {
    var minCost = new int[Grid.GetCells().Length];
    for (var i = 0; i < minCost.Length; i++) {
      minCost[i] = int.MaxValue;
    }

    minCost[Grid.GetCellIndex(start)] = 0;

    var reachableCells = new List<GameCell>();
    var pq = new PriorityQueue<GameCell, int>();
    pq.Enqueue(start, 0);

    while (pq.Count > 0) {
      var current = pq.Dequeue();
      var currentCost = minCost[Grid.GetCellIndex(current)];

      if (currentCost > maxCost) {
        continue;
      }

      reachableCells.Add(current);

      for (var i = HexDirection.Min; i < HexDirection.Max; i++) {
        var cell = GetCell(current, i);
        if (cell != null && isReachable(current, cell)) {
          var newCost = currentCost + GetSetting(cell).MoveCost;
          var cellIndex = Grid.GetCellIndex(cell);
          if (newCost < minCost[cellIndex]) {
            minCost[cellIndex] = newCost;
            pq.Enqueue(cell, newCost);
          }
        }
      }
    }

    return reachableCells.ToArray();
  }


  public GameCell[] FindPath(GameCell start, GameCell target, uint maxCost, Func<GameCell, GameCell, bool> isReachable) {
    var minCost = new int[Grid.GetCells().Length];
    var parent = new GameCell[Grid.GetCells().Length];

    for (var i = 0; i < minCost.Length; i++) {
      minCost[i] = int.MaxValue;
      parent[i] = null;
    }

    minCost[Grid.GetCellIndex(start)] = 0;

    var pq = new PriorityQueue<GameCell, int>();
    pq.Enqueue(start, 0);

    while (pq.Count > 0) {
      var current = pq.Dequeue();
      var currentCost = minCost[Grid.GetCellIndex(current)];

      if (current == target) {
        break;
      }

      for (var i = (int)HexDirection.Min; i < (int)HexDirection.Max; i++) {
        var cell = GetCell(current, (HexDirection)i);
        if (cell != null && isReachable(current, cell)) {
          var newCost = currentCost + GetSetting(cell).MoveCost;
          var cellIndex = Grid.GetCellIndex(cell);
          if (newCost < minCost[cellIndex]) {
            minCost[cellIndex] = newCost;
            parent[cellIndex] = current;
            pq.Enqueue(cell, newCost);
          }
        }
      }
    }

    if (minCost[Grid.GetCellIndex(target)] == int.MaxValue) {
      return null;
    }

    var path = new List<GameCell>();
    var currentCell = target;

    while (currentCell != null) {
      path.Add(currentCell);
      currentCell = parent[Grid.GetCellIndex(currentCell)];
    }

    path.Reverse();
    return path.ToArray();
  }

  public CellSetting GetSetting(GameCell cell) {
    return MapData.Settings.CellSettings[cell.SettingId];
  }

  private void LoadMap(MapData mapData) {
    var cells = new List<GameCell>();
    foreach (var hex in MapData.GetCells()) {
      var cell = new GameCell(Layout.OffsetToCube(new OffsetCoord(hex.Col, hex.Row)), Layout, hex.Id);
      cells.Add(cell);
    }

    Grid.CreateCells(cells.ToArray());
  }
}
