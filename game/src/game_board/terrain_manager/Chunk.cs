namespace HOB;

using Godot;
using HexGridMap;
using System;

public partial class Chunk : Node3D {
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

    AddChild(TerrainMesh);

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
  }

  private void Triangulate(int cellIndex) {
    var cell = Board.GetCell(cellIndex);
    if (cell == null) {
      return;
    }

    for (var d = HexDirection.Min; d < HexDirection.Max; d++) {
      Triangulate(d, cell, cellIndex);
    }
  }

  private void Triangulate(HexDirection direction, GameCell cell, int cellIndex) {
    var firstCorner = new Vector3(cell.GetCorner(direction).X, 0, cell.GetCorner(direction).Y);
    var secondCorner = new Vector3(cell.GetCorner(direction + 1).X, 0, cell.GetCorner(direction + 1).Y);
    var e = new EdgeVertices(firstCorner, secondCorner);


    TriangulateEdgeFan(new(cell.Position.X, 0, cell.Position.Y), e, cellIndex);
  }

  private void TriangulateEdgeFan(Vector3 pos, EdgeVertices edge, float index) {
    TerrainMesh.AddTriangle(pos, edge.V1, edge.V5);
  }
}
