namespace HOB;

using System.Collections.Generic;
using Godot;
using HOB.GameEntity;

/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
[Tool]
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();
  [Export] private GameGridLayout Layout { get; set; } = default!;
  [Export] private TerrainManager TerrainManager { get; set; } = default!;

  [Export]
  private bool Generate {
    get => false; set {
      if (MapData != null) {
        Init(MapData);
      }
    }
  }
  [Export]
  private MapData MapData { get; set; } = default!;
  public GameGrid Grid { get; private set; } = default!;

  public void Init(MapData mapData) {
    if (Layout == null) {
      return;
    }

    Grid = new(Layout);

    Grid.LoadMap(mapData);

    TerrainManager?.CreateData(Grid);

    foreach (var cell in Grid.GetCells()) {
      TerrainManager?.AddCellToChunk(cell);
    }

    EmitSignal(SignalName.GridCreated);
  }


  public Aabb GetAabb() {
    if (Grid == null) {
      return new();
    }

    var aabb = new Aabb {
      Size = new(Grid.GetRealMapSize().X, 1, Grid.GetRealMapSize().Y),
      // TODO: that offset probably will be wrong with different hex oreintations
      Position = new(-Grid.GetLayout().GetRealHexSize().X / 2, 0, -Grid.GetLayout().GetRealHexSize().Y / 2),
    };
    return aabb;
  }

  public void SetMouseHighlight(bool value) {
    TerrainManager?.SetMouseHighlight(value);
  }

  public void SetHighlight(GameCell cell, Color color) {
    TerrainManager?.SetHighlight(cell, color);
  }

  public void UpdateHighlights() {
    TerrainManager?.UpdateHighlights();
  }

  public void ClearHighlights() {
    TerrainManager?.ClearHighlights();
  }
}
