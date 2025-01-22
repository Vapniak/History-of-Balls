namespace HOB;

using Godot;
using HexGridMap;
using HOB.GameEntity;
using RaycastSystem;


/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();
  [Export] private MeshInstance3D _terrainMesh;
  [Export] private HexLayout Layout { get; set; }
  [Export] private GridShape GridShape { get; set; }


  private GameGrid Grid { get; set; }
  private EntityManager EntityManager { get; set; }
  private TerrainManager TerrainManager { get; set; }

  private Material _terrainMaterial;

  public void Init() {
    Grid = new(Layout, GridShape);

    EntityManager = new();

    TerrainManager = new();

    _terrainMaterial = _terrainMesh.GetActiveMaterial(0);

    // TODO: make terrain grid infinite
    ((PlaneMesh)_terrainMesh.Mesh).Size = Grid.GetRealSize() * 10;
    TerrainManager.TerrainDataTextureChanged += (tex) => _terrainMaterial.Set("shader_parameter/terrain_data_texture", tex);
    TerrainManager.HighlightDataTextureChanged += (tex) => _terrainMaterial.Set("shader_parameter/highlight_data_texture", tex);

    _terrainMaterial.Set("shader_parameter/terrain_size", Grid.GetRectSize());

    TerrainManager.CreateData(Grid.GetRectSize().X, Grid.GetRectSize().Y, GetCells());

    SetMouseHighlight(true);

    AddChild(TerrainManager);
    AddChild(EntityManager);

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
    var position = RaycastSystem.RaycastOnMousePosition(GetWorld3D(), GetViewport(), GameLayers.Physics3D.Mask.World)?.Position;
    if (position != null) {
      _terrainMaterial.Set("shader_parameter/mouse_world_pos", new Vector2(position.Value.X, position.Value.Z));
    }
  }

  public Aabb GetAabb() {
    var aabb = new Aabb {
      Size = new(Grid.GetRealSize().X, 1, Grid.GetRealSize().Y),
      // TODO: that offset probably will be wrong with different hex oreintations
      Position = new(-Grid.GetLayout().GetRealHexSize().X / 2, 0, -Grid.GetLayout().GetRealHexSize().Y / 2),
    };
    return aabb;
  }

  public void SetMouseHighlight(bool value) {
    _terrainMaterial.Set("shader_parameter/show_mouse_highlight", value);
  }

  public CubeCoord PointToCube(Vector3 point) {
    return Grid.GetLayout().PointToHex(new(point.X, point.Z));
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

  public GameCell[] GetCells(CubeCoord[] coords) {
    return Grid.GetCells(coords);
  }

  public GameCell[] GetCellsInRange(CubeCoord center, uint range) {
    return Grid.GetCellsInRange(center, range);
  }

  public void AddEntity(Entity entity, CubeCoord coord, IMatchController controller) {
    var cell = GetCell(coord);
    EntityManager.AddEntity(entity, cell, new(cell.Position.X, 0, cell.Position.Y), controller);
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
}
