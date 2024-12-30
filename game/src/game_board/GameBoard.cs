namespace HOB;

using Godot;
using HexGridMap;

public partial class GameBoard : Node3D {
  [Export] public HexGrid Grid { get; private set; }
  [Export] private PackedScene _debugMesh;

  public Vector3[] CellPositions { get; private set; }

  private Aabb _combinedAabb;
  public override void _Ready() {
    Grid.CreateGrid();

    var cells = Grid.GetCells();
    var count = cells.Length;
    CellPositions = new Vector3[count];

    _combinedAabb = new();

    // TODO: procedural map generation
    for (var i = 0; i < count; i++) {
      var cell = cells[i];
      var point = Grid.GetLayout().HexCoordinatesToPoint(cell.Coordinates);
      var mesh = _debugMesh.Instantiate<MeshInstance3D>();

      CellPositions[i] = new(point.X, 0, point.Y);

      mesh.Position = new(point.X, 0, point.Y);
      mesh.Scale = Vector3.One * Grid.GetLayout().HexCellScale;
      AddChild(mesh);

      _combinedAabb = _combinedAabb.Merge(mesh.GlobalTransform * mesh.GetAabb());
    }

    // TODO: hex units
  }
  public Aabb GetAabb() => _combinedAabb;
}
