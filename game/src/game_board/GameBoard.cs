namespace HOB;

using System;
using Godot;
using HexGridMap;
using HOB.GameEntity;

/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void EntityAddedEventHandler(Entity entity);
  [Signal] public delegate void EntityRemovedEventHandler(Entity entity);

  [Signal] public delegate void GridCreatedEventHandler();
  [Export] public MapData MapData { get; private set; }
  [Export] private GameGridLayout Layout { get; set; }
  [Export] private TerrainManager TerrainManager { get; set; }
  [Export] private EntityManager EntityManager { get; set; }

  public GameGrid Grid { get; private set; }


  public void Init() {
    Grid = new(Layout);

    EntityRemoved += (entity) => {
      EmitSignal(SignalName.EntityRemoved, entity);
      SetHighlight(entity.Cell, new());
      UpdateHighlights();
    };

    Grid.LoadMap(MapData);

    TerrainManager.CreateData(Grid);

    foreach (var cell in Grid.GetCells()) {
      TerrainManager.AddCellToChunk(cell);
    }

    EmitSignal(SignalName.GridCreated);
  }


  public Aabb GetAabb() {
    var aabb = new Aabb {
      Size = new(Grid.GetRealMapSize().X, 1, Grid.GetRealMapSize().Y),
      // TODO: that offset probably will be wrong with different hex oreintations
      Position = new(-Grid.GetLayout().GetRealHexSize().X / 2, 0, -Grid.GetLayout().GetRealHexSize().Y / 2),
    };
    return aabb;
  }

  public void SetMouseHighlight(bool value) {
    TerrainManager.SetMouseHighlight(value);
  }


  public void AddEntityOnClosestAvailableCell(EntityData data, CubeCoord coord, IMatchController owner) {
    GameCell closestCell = null;
    var minDistance = int.MaxValue;

    foreach (var cell in Grid.GetCells()) {
      var distance = coord.Distance(cell.Coord);
      if (distance < minDistance && CanEntityBePlacedOnCell(cell)) {
        minDistance = distance;
        closestCell = cell;
      }
    }

    var entity = new Entity(data, closestCell, this);

    AddEntity(entity, owner);
  }

  private bool CanEntityBePlacedOnCell(GameCell cell) {
    if (Grid.GetSetting(cell).IsWater) {
      return false;
    }

    foreach (var entity in GetEntitiesOnCell(cell)) {
      if (entity.TryGetTrait<ObstacleTrait>(out _)) {
        return false;
      }
    }

    return true;
  }

  public bool TryAddEntityOnCell(EntityData data, GameCell cell, IMatchController owner) {
    if (CanEntityBePlacedOnCell(cell)) {

      var entity = new Entity(data, cell, this);
      AddEntity(entity, owner);
      return true;
    }

    return false;
  }

  public Entity[] GetOwnedEntitiesOnCell(IMatchController owner, GameCell cell) {
    return EntityManager.GetOwnedEntitiesOnCell(owner, cell);
  }

  public Entity[] GetEntities() {
    return EntityManager.GetEntities();
  }

  public Entity[] GetEntitiesOnCell(GameCell cell) {
    return EntityManager.GetEntitiesOnCell(cell);
  }

  public Entity[] GetOwnedEntities(IMatchController owner) {
    return EntityManager.GetOwnedEntites(owner);
  }

  public Entity[] GetEnemyEntities(IMatchController controller) {
    return EntityManager.GetEnemyEntities(controller);
  }

  public Entity[] GetNotOwnedEntities() {
    return EntityManager.GetNotOwnedEntities();
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

  public Vector2I GetMapSize() => Grid.GetMapSize();

  private void AddEntity(Entity entity, IMatchController owner) {
    EntityManager.AddEntity(entity);
    entity.ChangeOwner(owner);
    EmitSignal(SignalName.EntityAdded, entity);
  }
}
