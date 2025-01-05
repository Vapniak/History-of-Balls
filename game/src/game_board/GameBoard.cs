namespace HOB;

using Godot;
using HexGridMap;

/// <summary>
/// Responsible for visualization and working with hex grid.
/// </summary>
public partial class GameBoard : Node3D {
  [Signal] public delegate void GridCreatedEventHandler();


  [Export] private HexGrid Grid { get; set; }
  [Export] private PackedScene _debugMesh;

  private Aabb _combinedAabb;
  public override void _Ready() {
    Grid.CreateGrid();

    var cells = Grid.GetCells();
    var count = cells.Length;

    _combinedAabb = new();

    // TODO: procedural map generation and divide it into chunks and optimalize
    // TODO: chunk loading of nearest visible nodes
    // TODO: option to load map from external file
    for (var i = 0; i < count; i++) {
      var cell = cells[i];
      var point = Grid.GetLayout().HexToPoint(cell);
      var mesh = _debugMesh.Instantiate<MeshInstance3D>();

      mesh.Position = new(point.X, 0, point.Y);
      AddChild(mesh);

      _combinedAabb = _combinedAabb.Merge(mesh.GlobalTransform * mesh.GetAabb());
    }

    EmitSignal(SignalName.GridCreated);
  }
  public Aabb GetAabb() => _combinedAabb;

  public HexCoordinates GetHexCoordinates(Vector3 point) {
    return Grid.GetLayout().PointToHex(new(point.X, point.Z));
  }

  public Vector3 GetPoint(HexCoordinates coordinates) {
    var point = Grid.GetLayout().HexToPoint(coordinates);
    return new(point.X, GetAabb().GetCenter().Y + (GetAabb().Size.Y / 2), point.Y);
  }
}
