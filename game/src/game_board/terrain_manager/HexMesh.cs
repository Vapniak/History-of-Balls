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
    Mesh = ST.Commit();
  }

  public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
    ST.AddVertex(v1);
    ST.AddVertex(v2);
    ST.AddVertex(v3);
  }
}
