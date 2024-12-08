namespace HexGridMap;

using Godot;

public partial class HexGridChunk : Node3D {
  public HexGrid Grid { get; set; }

  private HexMesh _terrain;

  private int[] _cellIndices;

  public override void _Ready() {
    _cellIndices = new int[HexUtils.CHUNK_SIZE_X * HexUtils.CHUNK_SIZE_Z];

    _terrain = new();
    AddChild(_terrain);
  }
  public override void _Process(double delta) {
    // TODO: find some better way, more decentrailzed way to triangulate
    Triangulate();
    SetProcess(false);
  }
  public void AddCell(int index, int cellIndex) {
    _cellIndices[index] = cellIndex;
  }

  public void Refresh() => SetProcess(true);

  public void Triangulate() {
    _terrain.Clear();
    for (var i = 0; i < _cellIndices.Length; i++) {
      Triangulate(_cellIndices[i]);
    }
    _terrain.Apply();
  }

  private void Triangulate(int cellIndex) {
    var cellData = Grid.CellData[cellIndex];
    var cellPosition = Grid.CellPositions[cellIndex];

    for (var d = HexDirection.NE; d <= HexDirection.NW; d++) {
      Triangulate(d, cellData, cellIndex, cellPosition);
    }
  }

  private void Triangulate(HexDirection direction, HexCellData data, int cellIndex, Vector3 center) {
    var e = new HexEdge(center + HexUtils.GetSolidCorner(direction), center + HexUtils.GetSolidCorner(direction.Next()));

    TriangulateEdgeFan(center, e, cellIndex);
  }

  private void TriangulateEdgeFan(Vector3 center, HexEdge edge, float index) {
    _terrain.AddTriangle(center, edge.V1, edge.V2);
    _terrain.AddTriangle(center, edge.V2, edge.V3);
    _terrain.AddTriangle(center, edge.V3, edge.V4);
    _terrain.AddTriangle(center, edge.V4, edge.V5);
  }
}
