namespace HOB;

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

    EntityManager.EntityRemoved += (entity) => {
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


  public bool TryAddEntity(EntityData data, CubeCoord coord, IMatchController controller) {
    GameCell closestCell = null;
    var minDistance = int.MaxValue;

    foreach (var cell in Grid.GetCells()) {
      var distance = coord.Distance(cell.Coord);
      if (distance < minDistance && GetEntitiesOnCell(cell).Length == 0 && Grid.GetSetting(cell).MoveCost > 0) {
        minDistance = distance;
        closestCell = cell;
      }
    }

    var entity = new Entity(data, closestCell, this);

    EntityManager.AddEntity(entity);

    entity.ChangeOwner(controller);

    EmitSignal(SignalName.EntityAdded, entity);
    return true;
  }

  public void SetMaterialsForEntity(Entity entity, IMatchController controller) {
    EntityManager.SetEntityMaterialBasedOnOwnership(entity.GetOwnershipType(controller), entity);

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

  public void SetHighlight(GameCell cell, HighlightType highlightType) {
    TerrainManager.SetHighlight(cell, highlightType);
  }

  public void AddHighlight(GameCell cell, HighlightType highlightType) {
    TerrainManager.AddHighlight(cell, highlightType);
  }

  public void UpdateHighlights() {
    TerrainManager.UpdateHighlights();
  }

  public void ClearHighlights() {
    TerrainManager.ClearHighlights();
  }

  public Vector2I GetMapSize() => Grid.GetMapSize();
}
