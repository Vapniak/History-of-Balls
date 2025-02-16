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
  [Export] public MapData MapData { get; private set; }
  [Export] private GameGridLayout Layout { get; set; }
  [Export] private TerrainManager TerrainManager { get; set; }
  [Export] private EntityManager EntityManager { get; set; }

  public GameGrid Grid { get; private set; }


  public void Init() {
    Grid = new(Layout);

    EntityManager.EntityRemoved += (entity) => {
      SetHighlight(entity.Cell, Colors.Transparent);
      UpdateHighlights();
    };

    Grid.LoadMap(MapData);

    TerrainManager.CreateData(Grid);

    foreach (var cell in Grid.GetCells()) {
      TerrainManager.AddCellToChunk(cell);
    }


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

  public Entity[] GetEnemyEntities(IMatchController controller) {
    return EntityManager.GetEnemyEntities(controller);
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
}
