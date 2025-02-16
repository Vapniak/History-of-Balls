namespace HOB;

using Godot;

public partial class HexMesh : MeshInstance3D {
  private SurfaceTool ST { get; set; }

  public override void _Ready() {
    Clear();
  }

  public void Clear() {
    ST = new();
    ST.Clear();
    ST.Begin(Mesh.PrimitiveType.Triangles);
  }

  public void Apply() {
    ST.Index();
    ST.GenerateNormals();
    ST.GenerateTangents();
    Mesh = ST.Commit();
  }

  public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
    ST.SetUV(new Vector2(v1.X, v1.Z).Normalized());
    ST.AddVertex(v1);
    ST.SetUV(new Vector2(v2.X, v2.Z).Normalized());
    ST.AddVertex(v2);
    ST.SetUV(new Vector2(v3.X, v3.Z).Normalized());
    ST.AddVertex(v3);
  }

  public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
    AddTriangle(v1, v3, v2);
    AddTriangle(v2, v3, v4);
  }
}
