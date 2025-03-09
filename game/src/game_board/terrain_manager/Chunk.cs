namespace HOB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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


  private GameGrid Grid { get; set; }

  private HexMesh TerrainMesh { get; set; }
  private HexMesh WaterMesh { get; set; }
  private CollisionShape3D CollisionShape { get; set; }


  private HexMesh _borderMesh;
  private int _borderLayers = 1;

  private bool _refresh;

  public Chunk(int index, Vector2I chunkSize, Material terrainMaterial, Material waterMaterial, GameGrid grid) {
    Index = index;
    ChunkSize = chunkSize;
    Grid = grid;

    CellIndices = new int[chunkSize.X * chunkSize.Y];

    TerrainMesh = new() {
      MaterialOverride = terrainMaterial,
    };

    WaterMesh = new() {
      MaterialOverride = waterMaterial
    };

    CollisionShape = new();

    AddChild(CollisionShape);
    CollisionShape.AddChild(TerrainMesh);
    AddChild(WaterMesh);

    _borderMesh = new HexMesh {
      MaterialOverride = terrainMaterial
    };
    AddChild(_borderMesh);

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
    TerrainMesh.Begin();
    WaterMesh.Begin();

    foreach (var index in CellIndices) {
      Triangulate(index);
    }

    GenerateHexBorderRectangle(10);

    TerrainMesh.End();
    WaterMesh.End();

    CollisionShape.Shape = TerrainMesh.Mesh.CreateTrimeshShape();
  }

  private void Triangulate(int cellIndex) {
    var cell = Grid.GetCell(cellIndex);
    if (cell == null) {
      return;
    }

    for (var d = HexDirection.First; d <= HexDirection.Sixth; d++) {
      Triangulate(d, cell);
    }
  }

  private void Triangulate(HexDirection direction, GameCell cell) {
    var pos = Grid.GetCellRealPosition(cell);
    var (firstCorner, secondCorner) = Grid.GetCorners(direction);
    var e = new EdgeVertices(pos + firstCorner, pos + secondCorner);

    TriangulateEdgeFan(pos, e);

    if (direction <= HexDirection.Third) {
      TriangulateConnection(direction, cell, e);
    }

    if (cell.GetSetting().IsWater) {
      TriangulateWater(direction, cell);

      if (Grid.GetCell(cell, direction) == null) {
        var wallTop1 = e.V1;
        var wallTop2 = e.V5;
        var wallBottom1 = wallTop1 with { Y = Grid.GetLayout().WaterLevel };
        var wallBottom2 = wallTop2 with { Y = Grid.GetLayout().WaterLevel };

        TerrainMesh.AddQuadAutoUV(
            wallTop1,
            wallTop2,
            wallBottom1,
            wallBottom2
        );

        TerrainMesh.AddQuadAutoUV(
            wallBottom1,
            wallBottom2,
            wallTop1,
            wallTop2
        );
      }
    }
  }

  private void TriangulateEdgeFan(Vector3 pos, EdgeVertices edge) {
    TerrainMesh.AddTriangleAutoUV(pos, edge.V1, edge.V5);
  }

  private void TriangulateConnection(HexDirection direction, GameCell cell, EdgeVertices e) {
    var neighbor = Grid.GetCell(cell, direction);

    if (neighbor == null) {
      var wallTop1 = e.V1;
      var wallTop2 = e.V5;
      var wallBottom1 = wallTop1 with { Y = 0 };
      var wallBottom2 = wallTop2 with { Y = 0 };

      TerrainMesh.AddQuadAutoUV(
          wallTop1,
          wallTop2,
          wallBottom1,
          wallBottom2
      );

      TerrainMesh.AddQuadAutoUV(
          wallBottom1,
          wallBottom2,
          wallTop1,
          wallTop2
      );
      return;
    }

    var bridge = Grid.GetBridge(direction);
    bridge.Y = Grid.GetCellRealPosition(neighbor).Y - Grid.GetCellRealPosition(cell).Y;
    var e2 = new EdgeVertices(e.V1 + bridge, e.V5 + bridge);



    var edgeType = Grid.GetEdgeType(cell, neighbor);
    if (edgeType == GameCell.EdgeType.Slope) {
      TriangulateEdgeTerraces(e, e2);
    }
    else {
      TriangulateEdgeStrip(e, e2);
    }

    if (direction <= HexDirection.Second) {
      var nextNeighbor = Grid.GetCell(cell, direction.Next());
      if (nextNeighbor == null) {
        return;
      }

      var v5 = e.V5 + Grid.GetBridge(direction.Next());
      v5.Y = Grid.GetCellRealPosition(nextNeighbor).Y;


      if (Grid.GetSetting(cell).Elevation <= Grid.GetSetting(neighbor).Elevation) {
        if (Grid.GetSetting(cell).Elevation <= Grid.GetSetting(nextNeighbor).Elevation) {
          TriangulateCorner(
            e.V5, cell,
            e2.V5, neighbor,
            v5, nextNeighbor);
        }
        else {
          TriangulateCorner(
            v5, nextNeighbor,
            e.V5, cell,
            e2.V5, neighbor);
        }
      }
      else if (Grid.GetSetting(neighbor).Elevation <= Grid.GetSetting(nextNeighbor).Elevation) {
        TriangulateCorner(
          e2.V5, neighbor,
          v5, nextNeighbor,
          e.V5, cell);
      }
      else {
        TriangulateCorner(
          v5, nextNeighbor,
          e.V5, cell,
          e2.V5, neighbor);
      }
    }
  }

  private void TriangulateEdgeStrip(EdgeVertices e1, EdgeVertices e2) {
    TerrainMesh.AddQuadAutoUV(e1.V1, e1.V5, e2.V1, e2.V5);
  }

  private void TriangulateEdgeTerraces(EdgeVertices begin, EdgeVertices end) {
    var e2 = Grid.TerraceLerp(begin, end, 1);

    TriangulateEdgeStrip(begin, e2);

    for (var i = 2; i < Grid.GetLayout().TerraceSteps; i++) {
      var e1 = e2;
      e2 = Grid.TerraceLerp(begin, end, i);
      TriangulateEdgeStrip(e1, e2);
    }

    TriangulateEdgeStrip(e2, end);
  }

  private void TriangulateCorner(
    Vector3 bottom, GameCell bottomCell,
    Vector3 left, GameCell leftCell,
    Vector3 right, GameCell rightCell) {
    var leftEdgeType = Grid.GetEdgeType(bottomCell, leftCell);
    var rightEdgeType = Grid.GetEdgeType(bottomCell, rightCell);

    TerrainMesh.AddTriangleAutoUV(bottom, left, right);
  }

  private void TriangulateWater(HexDirection direction, GameCell cell) {
    var pos = cell.GetRealPosition();
    pos.Y = Grid.GetLayout().WaterLevel;

    var neighbor = Grid.GetCell(cell, direction);

    if (neighbor != null && !neighbor.GetSetting().IsWater) {
      // shore
    }
    else {

      // open water
    }
    // for now make the water take whole hex
    TriangulateOpenWater(direction, pos, cell, neighbor);
  }

  private void TriangulateOpenWater(HexDirection direction, Vector3 pos, GameCell cell, GameCell neighbor) {
    var (firstCorner, secondCorner) = Grid.GetWaterCorners(direction);
    firstCorner += pos;
    secondCorner += pos;

    WaterMesh.AddTriangleAutoUV(pos, firstCorner, secondCorner);

    if (direction <= HexDirection.Third && neighbor != null) {
      var bridge = Grid.GetWaterBridge(direction);
      var e1 = firstCorner + bridge;
      var e2 = secondCorner + bridge;
      WaterMesh.AddQuadAutoUV(firstCorner, secondCorner, e1, e2);

      if (direction <= HexDirection.Second) {
        var nextNeigbor = Grid.GetCell(cell, direction.Next());
        if (nextNeigbor == null || !nextNeigbor.GetSetting().IsWater) {
          return;
        }

        WaterMesh.AddTriangleAutoUV(secondCorner, e2, secondCorner + Grid.GetWaterBridge(direction.Next()));
      }
    }
  }
  private void GenerateHexBorderRectangle(int borderLayers) {
    var originalCells = new HashSet<OffsetCoord>(
        CellIndices.Select(i => Grid.GetCell(i).OffsetCoord)
                   .Where(oc => oc.Col >= 0 && oc.Row >= 0)
    );

    if (originalCells.Count == 0) {
      return;
    }

    var minCol = originalCells.Min(oc => oc.Col);
    var maxCol = originalCells.Max(oc => oc.Col);
    var minRow = originalCells.Min(oc => oc.Row);
    var maxRow = originalCells.Max(oc => oc.Row);

    var expandedMinCol = minCol - borderLayers;
    var expandedMaxCol = maxCol + borderLayers;
    var expandedMinRow = minRow - borderLayers;
    var expandedMaxRow = maxRow + borderLayers;

    _borderMesh.Begin();

    for (var col = expandedMinCol; col <= expandedMaxCol; col++) {
      for (var row = expandedMinRow; row <= expandedMaxRow; row++) {
        var offsetCoord = new OffsetCoord(col, row);
        if (!originalCells.Contains(offsetCoord)) {
          var cubeCoord = Grid.GetLayout().OffsetToCube(offsetCoord);
          var point = Grid.GetLayout().CubeToPoint(cubeCoord);
          var position = new Vector3(point.X, 0, point.Y);

          GenerateBorderHexagon(position);
        }
      }
    }

    _borderMesh.End();
  }

  private void GenerateBorderHexagon(Vector3 center) {
    var vertices = new List<Vector3>();

    for (var i = 0; i < 6; i++) {
      var corner = Grid.GetLayout().GetCorner(i);
      vertices.Add(center + new Vector3(corner.X, 0, corner.Y));
    }

    for (var i = 0; i < 6; i++) {
      _borderMesh.AddTriangleAutoUV(
          center,
          vertices[i],
          vertices[(i + 1) % 6]
      );
    }

    var wallHeight = -2f;
    for (var i = 0; i < 6; i++) {
      var top1 = vertices[i];
      var top2 = vertices[(i + 1) % 6];
      var bottom1 = top1 with { Y = wallHeight };
      var bottom2 = top2 with { Y = wallHeight };

      _borderMesh.AddQuadAutoUV(top1, top2, bottom1, bottom2);
    }
  }
}
