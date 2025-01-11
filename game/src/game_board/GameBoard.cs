namespace HOB;

using Godot;
using HexGridMap;

/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();
  [Export] private HexGrid Grid { get; set; }
  [Export] private MeshInstance3D _gridMesh;

  public EntityManager EntityManager { get; private set; }
  public override void _Ready() {
    EntityManager = new() {
      GameBoard = this
    };
    AddChild(EntityManager);

    // TODO: procedural map generation and divide it into chunks and optimalize
    // TODO: chunk loading of nearest visible nodes
    // TODO: option to load map from external file

    // TODO: work on shader for hex cell
    ((PlaneMesh)_gridMesh.Mesh).Size = Grid.GetRectSize();
    EmitSignal(SignalName.GridCreated);
  }

  public override void _PhysicsProcess(double delta) {
    // DebugDraw3D.DrawAabb(GetAabb(), Colors.Red);
  }
  public Aabb GetAabb() {
    var aabb = new Aabb();


    // TODO: find better solution for this
    aabb.Size = new(Grid.GetRectSize().X, 1, Grid.GetRectSize().Y);
    aabb.Position = new(-Grid.GetRectSize().X / 2, 0, -Grid.GetRectSize().Y / 2);
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
