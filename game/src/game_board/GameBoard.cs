namespace HOB;

using GameplayFramework;
using Godot;

/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();
  [Export] public MapData MapData { get; private set; }
  [Export] private GameGridLayout Layout { get; set; }
  [Export] private TerrainManager TerrainManager { get; set; }

  public GameGrid Grid { get; private set; }

  public void Init() {
    Grid = new(Layout);

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

  public void SetHighlight(GameCell cell, Color color) {
    TerrainManager.SetHighlight(cell, color);
  }

  public void UpdateHighlights() {
    TerrainManager.UpdateHighlights();
  }

  public void ClearHighlights() {
    TerrainManager.ClearHighlights();
  }
}
