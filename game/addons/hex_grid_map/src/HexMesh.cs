namespace HexGridMap;


using Godot;
using Godot.Collections;
using System;
using System.Data.SqlTypes;

public partial class HexMesh : MeshInstance3D {
  private Array<Vector3> _vertices;

  private Mesh _hexMesh;
  private CollisionShape3D _collisionShape;

  public override void _Ready() {
    Mesh = _hexMesh = new ArrayMesh();
  }

  public void Clear() {
    _hexMesh = new ArrayMesh();
    _vertices = new();
  }

  public void Apply() {
    SurfaceTool st = new();
    st.Begin(Mesh.PrimitiveType.Triangles);

    // FIX: TEMP COLOR
    st.SetColor(Colors.White);
    foreach (var vertex in _vertices) {
      st.AddVertex(vertex);
    }
    _hexMesh = st.Commit();
  }

  public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
    var vertexIndex = _vertices.Count;
    _vertices.Add(HexUtils.Perturb(v1));
    _vertices.Add(HexUtils.Perturb(v2));
    _vertices.Add(HexUtils.Perturb(v3));
  }
}
