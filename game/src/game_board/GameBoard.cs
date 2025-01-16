namespace HOB;

using Godot;
using HexGridMap;
using HOB.GameEntity;


/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();
  [Export] private HexGrid Grid { get; set; }
  [Export] private MeshInstance3D _terrainMesh;


  private EntityManager EntityManager { get; set; }
  private TerrainManager TerrainManager { get; set; }

  public override void _Ready() {
    Grid.CreateGrid();


    EntityManager = new();

    TerrainManager = new();

    // TODO: make terrain grid infinite
    ((PlaneMesh)_terrainMesh.Mesh).Size = Grid.GetRealSize() * 10;
    TerrainManager.TerrainDataTextureChanged += (tex) => _terrainMesh.GetActiveMaterial(0).Set("shader_parameter/terrain_data_texture", tex);
    TerrainManager.HighlightDataTextureChanged += (tex) => _terrainMesh.GetActiveMaterial(0).Set("shader_parameter/highlight_data_texture", tex);

    _terrainMesh.GetActiveMaterial(0).Set("shader_parameter/terrain_size", Grid.GetRectSize());

    TerrainManager.CreateData(Grid.GetRectSize().X, Grid.GetRectSize().Y);

    AddChild(TerrainManager);
    AddChild(EntityManager);

    EmitSignal(SignalName.GridCreated);


    // TODO: add console and command for showing debug
    // #if DEBUG
    //     DebugDraw3D.DrawAabb(GetAabb(), Colors.Red, float.MaxValue);

    //     foreach (var cell in GetCells()) {
    //       DebugDraw3D.DrawSphere(new(cell.GetPoint().X, 0, cell.GetPoint().Y), 0.5f, Colors.Blue, float.MaxValue);
    //     }
    // #endif
  }
  public Aabb GetAabb() {
    var aabb = new Aabb {
      Size = new(Grid.GetRealSize().X, 1, Grid.GetRealSize().Y),
      // TODO: that offset probably will be wrong with different hex oreintations
      Position = new(-Grid.GetLayout().GetRealHexSize().X / 2, 0, -Grid.GetLayout().GetRealHexSize().Y / 2),
    };
    return aabb;
  }

  public CubeCoord GetCubeCoord(Vector3 point) {
    return Grid.GetLayout().PointToHex(new(point.X, point.Z));
  }

  public Vector3 GetPoint(CubeCoord coord) {
    var point = Grid.GetLayout().HexToPoint(coord);
    return new(point.X, 0, point.Y);
  }

  public OffsetCoord CubeToOffset(CubeCoord coord) {
    return Grid.GetLayout().HexToOffset(coord);
  }

  public CubeCoord OffsetToCube(OffsetCoord offsetCoord) {
    return Grid.GetLayout().OffsetToHex(offsetCoord);
  }

  public HexCell[] GetCells() {
    return Grid.GetCells();
  }

  public HexCell[] GetCells(CubeCoord[] coords) {
    return Grid.GetCells(coords);
  }

  public void AddEntity(Entity entity, CubeCoord coord, IMatchController controller) {
    EntityManager.AddEntity(entity, coord, GetPoint(coord), controller);
  }

  public Entity[] GetOwnedEntitiesOnCoord(IMatchController owner, CubeCoord coord) {
    return EntityManager.GetOwnedEntitiesOnCoords(owner, coord);
  }

  public void AddHighlightToCoords(CubeCoord[] coords) {
    TerrainManager.AddHighlightToCells(GetCells(coords));
    TerrainManager.UpdateHighlights();
  }
}
