namespace HOB;

using Godot;
using HexGridMap;
using HOB.GameEntity;

public partial class GameBoard : Node3D {
  [Export] private HexGrid Grid { get; set; }
  [Export] private PackedScene _debugMesh;

  public Vector3[] CellPositions { get; private set; }

  private Aabb _combinedAabb;
  public override void _Ready() {
    Grid.CreateGrid();

    var cells = Grid.GetCells();
    var count = cells.Length;
    CellPositions = new Vector3[count];

    _combinedAabb = new();

    // TODO: procedural map generation and divide it into chunks and optimalize
    // TODO: chunk loading of nearest visible nodes
    // TODO: option to load map from external file
    for (var i = 0; i < count; i++) {
      var cell = cells[i];
      var point = Grid.GetLayout().HexToPoint(cell);
      var mesh = _debugMesh.Instantiate<MeshInstance3D>();

      CellPositions[i] = new(point.X, 0, point.Y);

      mesh.Position = new(point.X, 0, point.Y);
      AddChild(mesh);

      _combinedAabb = _combinedAabb.Merge(mesh.GlobalTransform * mesh.GetAabb());
    }
  }
  public Aabb GetAabb() => _combinedAabb;

  // TODO: cell selection and showing actions

  public HexCoordinates GetHexCoordinates(Vector3 point) {
    return Grid.GetLayout().PointToHex(new(point.X, point.Z));
  }

  public Vector3 GetPoint(HexCoordinates coordinates) {
    var point = Grid.GetLayout().HexToPoint(coordinates);
    return new(point.X, GetAabb().Size.Y, point.Y);
  }


  // FIXME: temp
  // TODO: better spawning
  public void SpawnEntity(Entity entity, HexCoordinates coordinates) {
    AddChild(entity);
    entity.Position = GetPoint(coordinates);
  }
}
