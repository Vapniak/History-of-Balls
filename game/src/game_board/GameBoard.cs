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
  [Export] private MeshInstance3D TerrainMesh { get; set; }
  [Export] private HexLayout Layout { get; set; }
  [Export] private TerrainManager TerrainManager { get; set; }
  [Export] private EntityManager EntityManager { get; set; }
  [Export] public MapData MapData { get; private set; }

  private GameGrid Grid { get; set; }

  private Material _terrainMaterial;

  public void Init() {
    Grid = new(Layout);

    EntityManager.EntityRemoved += (entity) => {
      entity.Cell.HighlightColor = Colors.Transparent;
      UpdateHighlights();
    };

    LoadMap(MapData);

    _terrainMaterial = TerrainMesh.GetActiveMaterial(0);

    // TODO: make terrain grid infinite
    ((PlaneMesh)TerrainMesh.Mesh).Size = GetRealMapSize() * 10;
    TerrainManager.TerrainDataTextureChanged += (tex) => _terrainMaterial.Set("shader_parameter/terrain_data_texture", tex);
    TerrainManager.HighlightDataTextureChanged += (tex) => _terrainMaterial.Set("shader_parameter/highlight_data_texture", tex);

    _terrainMaterial.Set("shader_parameter/terrain_size", GetMapSize());

    TerrainManager.CreateData(GetMapSize().X, GetMapSize().Y);

    TerrainManager.UpdateData(GetCells());

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

    var position = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World)?.Position;
    if (position != null) {
      _terrainMaterial.Set("shader_parameter/mouse_world_pos", new Vector2(position.Value.X, position.Value.Z));
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
    _terrainMaterial.Set("shader_parameter/show_mouse_highlight", value);
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

  public bool TryAddEntity(Entity entity, CubeCoord coord, IMatchController controller) {
    GameCell closestCell = null;
    var minDistance = int.MaxValue;

    foreach (var cell in GetCells()) {
      var distance = coord.Distance(cell.Coord);
      if (distance < minDistance && GetEntitiesOnCell(cell).Length == 0 && cell.MoveCost > 0) {
        minDistance = distance;
        closestCell = cell;
      }
    }

    EntityManager.AddEntity(entity, closestCell, controller);
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

  public void UpdateHighlights() {
    TerrainManager.UpdateHighlights();
  }

  public void ClearHighlights() {
    foreach (var cell in GetCells()) {
      cell.HighlightColor = Colors.Transparent;
    }
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
          var newCost = currentCost + cell.MoveCost;
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
          var newCost = currentCost + cell.MoveCost;
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

  private void LoadMap(MapData mapData) {
    var cells = new List<GameCell>();
    foreach (var hex in MapData.Cells) {
      var setting = mapData.Settings.CellDefinitions[hex.Id];
      var cell = new GameCell(Layout.OffsetToCube(new OffsetCoord(hex.Col, hex.Row)), Layout) {
        TerrainColor = setting.Color,
        MoveCost = setting.MoveCost
      };
      cells.Add(cell);
    }

    Grid.CreateCells(cells.ToArray());
  }
}
