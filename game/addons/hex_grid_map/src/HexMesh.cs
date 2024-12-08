namespace HexGridMap;

using Godot;
using Godot.Collections;

public partial class HexMesh : MeshInstance3D {
  private Array<Vector3> _vertices = new();
  private Array<int> _triangles = new();

  private ArrayMesh _hexMesh = new();
  private CollisionShape3D _collisionShape;

  public void Clear() {
    _hexMesh = new ArrayMesh();
    _vertices = new();
    _triangles = new();
  }

  public void Apply() {
    var st = new SurfaceTool();
    st.Begin(Mesh.PrimitiveType.Triangles);

    _vertices.Reverse();
    for (var i = 0; i < _vertices.Count; i++) {
      st.AddVertex(_vertices[i]);
    }

    st.Commit(_hexMesh);

    SetMesh(_hexMesh);
  }

  public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
    var vertexIndex = _vertices.Count;

    _vertices.Add(HexUtils.Perturb(v1));
    _vertices.Add(HexUtils.Perturb(v2));
    _vertices.Add(HexUtils.Perturb(v3));

    _triangles.Add(vertexIndex);
    _triangles.Add(vertexIndex + 1);
    _triangles.Add(vertexIndex + 2);
  }
}
