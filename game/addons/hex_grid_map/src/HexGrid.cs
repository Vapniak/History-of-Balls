namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class HexGrid : Node3D {

  [Export] private int _seed;
  [Export] private NoiseTexture2D _noiseSource;

  public int CellCountX { get; private set; }
  public int CellCountZ { get; private set; }

  public HexCellData[] CellData { get; private set; }
  public Vector3[] CellPositions { get; private set; }

  private HexGridChunk[] _chunks;

  private int _chunkCountX, _chunkCountZ;

  public override void _Ready() {
    CreateMap(16, 16);

    HexUtils.NOISE_SOURCE = _noiseSource;
  }

  public bool CreateMap(int sizeX, int sizeZ) {
    if (
      sizeX <= 0 || sizeX % HexUtils.CHUNK_SIZE_X != 0 ||
      sizeZ <= 0 || sizeZ % HexUtils.CHUNK_SIZE_Z != 0
    ) {
      GD.PrintErr("Invalid map size");
      return false;
    }


    CellCountX = sizeX;
    CellCountZ = sizeZ;
    _chunkCountX = CellCountX / HexUtils.CHUNK_SIZE_X;
    _chunkCountZ = CellCountZ / HexUtils.CHUNK_SIZE_Z;

    CreateChunks();
    CreateCells();
    return true;
  }

  private void CreateChunks() {
    _chunks = new HexGridChunk[_chunkCountX * _chunkCountZ];
    for (int z = 0, i = 0; z < _chunkCountZ; z++) {
      for (var x = 0; x < _chunkCountX; x++) {
        var chunk = _chunks[i++] = new();
        chunk.Grid = this;
        AddChild(chunk);
      }
    }
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

    var chunkX = x / HexUtils.CHUNK_SIZE_X;
    var chunkZ = z / HexUtils.CHUNK_SIZE_Z;
    var chunk = _chunks[chunkX + (chunkZ * _chunkCountX)];

    var localX = x - (chunkX * HexUtils.CHUNK_SIZE_X);
    var localZ = z - (chunkZ * HexUtils.CHUNK_SIZE_Z);
    chunk.AddCell(localX + (localZ * HexUtils.CHUNK_SIZE_X), i);
  }
}
