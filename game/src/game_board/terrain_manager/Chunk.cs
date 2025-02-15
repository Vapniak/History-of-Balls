namespace HOB;

using System.Diagnostics;
using Godot;
using HexGridMap;

public partial class Chunk : StaticBody3D {
  public struct EdgeVertices {
    public Vector3 V1, V2, V3, V4, V5;

    public EdgeVertices(Vector3 corner1, Vector3 corner2) {
      V1 = corner1;
      V2 = corner1.Lerp(corner2, 0.25f);
      V3 = corner1.Lerp(corner2, 0.5f);
      V4 = corner1.Lerp(corner2, 0.75f);
      V5 = corner2;
    }
  }
  public int Index { get; private set; }

  private Vector2I ChunkSize { get; set; }
  private int[] CellIndices { get; set; }

  private Material TerrainMaterial { get; set; }
  private GameBoard Board { get; set; }

  private HexMesh TerrainMesh { get; set; }
  private CollisionShape3D CollisionShape { get; set; }

  private bool _refresh;

  public Chunk(int index, Vector2I chunkSize, Material terrainMaterial, GameBoard board) {
    Index = index;
    ChunkSize = chunkSize;
    TerrainMaterial = terrainMaterial;
    Board = board;

    CellIndices = new int[chunkSize.X * chunkSize.Y];

    TerrainMesh = new() {
      MaterialOverride = terrainMaterial,
    };

    CollisionShape = new();

    AddChild(CollisionShape);
    CollisionShape.AddChild(TerrainMesh);

    Refresh();
  }

  public override void _PhysicsProcess(double delta) {
    if (_refresh) {
      _refresh = false;
      Triangulate();
    }
  }

  public void Refresh() => _refresh = true;

  public void AddCell(int localIndex, int cellIndex) {
    CellIndices[localIndex] = cellIndex;
  }

  private void Triangulate() {
    TerrainMesh.Clear();

    foreach (var index in CellIndices) {
      Triangulate(index);
    }

    TerrainMesh.Apply();

    CollisionShape.Shape = TerrainMesh.Mesh.CreateTrimeshShape();
  }

  private void Triangulate(int cellIndex) {
    var cell = Board.GetCell(cellIndex);
    if (cell == null) {
      return;
    }

    for (var d = HexDirection.First; d <= HexDirection.Sixth; d++) {
      Triangulate(d, cell);
    }
  }

  private void Triangulate(HexDirection direction, GameCell cell) {
    var pos = Board.GetCellRealPosition(cell);
    var (firstCorner, secondCorner) = Board.GetSolidCorners(direction);
    var e = new EdgeVertices(pos + firstCorner, pos + secondCorner);

    TriangulateEdgeFan(pos, e);

    if (direction <= HexDirection.Third) {
      TriangulateConnection(direction, cell, e);
    }
  }

  private void TriangulateEdgeFan(Vector3 pos, EdgeVertices edge) {
    TerrainMesh.AddTriangle(pos, edge.V1, edge.V5);
  }

  private void TriangulateConnection(HexDirection direction, GameCell cell, EdgeVertices e) {
    var neighbor = Board.GetCell(cell, direction);
    if (neighbor == null) {
      return;
    }

    var bridge = Board.GetBridge(direction);
    bridge.Y = Board.GetCellRealPosition(neighbor).Y - Board.GetCellRealPosition(cell).Y;
    var e2 = new EdgeVertices(e.V1 + bridge, e.V5 + bridge);



    var edgeType = Board.GetEdgeType(cell, neighbor);
    if (edgeType == GameCell.EdgeType.Slope) {
      TriangulateEdgeTerraces(e, e2);
    }
    else {
      TriangulateEdgeStrip(e, e2);
    }
  }

  private void TriangulateEdgeStrip(EdgeVertices e1, EdgeVertices e2) {
    TerrainMesh.AddQuad(e1.V1, e1.V5, e2.V1, e2.V5);
  }

  private void TriangulateEdgeTerraces(EdgeVertices begin, EdgeVertices end) {
    var e2 = Board.TerraceLerp(begin, end, 1);

    TriangulateEdgeStrip(begin, e2);

    for (var i = 2; i < Board.TerraceSteps; i++) {
      var e1 = e2;
      e2 = Board.TerraceLerp(begin, end, i);
      TriangulateEdgeStrip(e1, e2);
    }

    TriangulateEdgeStrip(e2, end);
  }
}
