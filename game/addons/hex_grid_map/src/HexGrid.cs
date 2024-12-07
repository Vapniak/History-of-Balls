namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class HexGrid : Node3D {

  [Export] private int _seed;

  public int CellCountX { get; private set; }
  public int CellCountZ { get; private set; }

  public HexCellData[] CellData { get; private set; }

  public Vector3[] CellPositions { get; private set; }

  public override void _Ready() {
    CreateMap(20, 20);

    GD.Print(CellData.Length);
  }

  public bool CreateMap(int sizeX, int sizeZ) {
    CellCountX = sizeX;
    CellCountZ = sizeZ;

    CreateCells();
    return true;
  }

  private void CreateCells() {
    CellData = new HexCellData[CellCountX * CellCountZ];
    CellPositions = new Vector3[CellData.Length];

    for (int z = 0, i = 0; z < CellCountZ; z++) {
      for (var x = 0; x < CellCountX; x++) {
        CreateCell(x, z, i++);
      }
    }
  }

  private void CreateCell(int x, int z, int i) {
    Vector3 position;
    position.X = (x + (z * 0.5f) - (z / 2)) * HexUtils.INNER_DIAMETER;
    position.Y = 0f;
    position.Z = z * (HexUtils.OUTER_RADIUS * 1.5f);

    CellPositions[i] = position;
    CellData[i].Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
  }
}
