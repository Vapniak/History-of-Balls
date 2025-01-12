namespace HOB;

using Godot;
using HexGridMap;

/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();
  [Export] private HexGrid Grid { get; set; }
  [Export] private MeshInstance3D _terrainMesh;

  public EntityManager EntityManager { get; private set; }

  private TerrainManager TerrainManager { get; set; }

  public override void _Ready() {
    EntityManager = new() {
      GameBoard = this
    };

    TerrainManager = new() {
      GameBoard = this
    };

    TerrainManager.CreateData(Grid.GetRectSize().X, Grid.GetRectSize().Y);


    AddChild(TerrainManager);
    AddChild(EntityManager);

    // TODO: procedural map generation and divide it into chunks and optimalize
    // TODO: chunk loading of nearest visible nodes
    // TODO: option to load map from external file

    ((PlaneMesh)_terrainMesh.Mesh).Size = Grid.GetRealSize() * 10;
    _terrainMesh.GetActiveMaterial(0).Set("shader_parameter/terrain_data", TerrainManager.TerrainDataTexture);
    _terrainMesh.GetActiveMaterial(0).Set("shader_parameter/terrain_size", new Vector2(Grid.GetRectSize().X, Grid.GetRectSize().Y));


    EmitSignal(SignalName.GridCreated);
  }

  public override void _PhysicsProcess(double delta) {
    DebugDraw3D.DrawAabb(GetAabb(), Colors.Red);
  }
  public Aabb GetAabb() {
    var aabb = new Aabb();


    // TODO: find better solution for this
    aabb.Size = new(Grid.GetRealSize().X, 1, Grid.GetRealSize().Y);
    aabb.Position = new(-Grid.GetRealSize().X / 2, 0, -Grid.GetRealSize().Y / 2);
    return aabb;
  }

  public HexCoordinates GetHexCoordinates(Vector3 point) {
    return Grid.GetLayout().PointToHex(new(point.X, point.Z));
  }

  public Vector3 GetPoint(HexCoordinates coordinates) {
    var point = Grid.GetLayout().HexToPoint(coordinates);
    return new(point.X, 0, point.Y);
  }
}
