namespace HOB;

using Godot;

public partial class HexMesh : MeshInstance3D {
  private SurfaceTool _surfaceTool;
  private float _uvScale = 0.1f;
  private Vector2 _uvOffset = Vector2.Zero;

  // Configuration structure for mesh generation
  public struct MeshConfig {
    public bool GenerateNormals;
    public bool GenerateTangents;
    public bool UseFlatShading;
    public float DefaultUVScale;
    public Vector2 DefaultUVOffset;
  }

  private MeshConfig _currentConfig = new() {
    GenerateNormals = true,
    GenerateTangents = true,
    UseFlatShading = false,
    DefaultUVScale = 0.1f,
    DefaultUVOffset = Vector2.Zero
  };

  public struct MeshVertex {
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 UV;
    public Plane Tangent;

    public MeshVertex(Vector3 position, Vector3 normal, Vector2 uv, Plane tangent) {
      Position = position;
      Normal = normal;
      UV = uv;
      Tangent = tangent;
    }
  }

  public HexMesh() {
    _surfaceTool = new SurfaceTool();
  }

  public void Configure(MeshConfig config) {
    _currentConfig = config;
    _uvScale = config.DefaultUVScale;
    _uvOffset = config.DefaultUVOffset;
  }

  public void Begin() {
    _surfaceTool.Clear();
    _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

    if (_currentConfig.UseFlatShading) {
      _surfaceTool.SetSmoothGroup(uint.MaxValue);
    }
  }

  public void End() {
    if (_currentConfig.GenerateNormals) {
      _surfaceTool.GenerateNormals();
    }

    if (_currentConfig.GenerateTangents) {
      _surfaceTool.GenerateTangents();
    }

    _surfaceTool.Index();
    Mesh = _surfaceTool.Commit();
  }

  public void AddTriangle(MeshVertex v1, MeshVertex v2, MeshVertex v3) {
    AddVertex(v1);
    AddVertex(v2);
    AddVertex(v3);
  }

  public void AddQuad(MeshVertex v1, MeshVertex v2, MeshVertex v3, MeshVertex v4) {
    AddTriangle(v1, v3, v2);
    AddTriangle(v2, v3, v4);
  }

  public void AddTriangleAutoUV(Vector3 v1, Vector3 v2, Vector3 v3) {
    AddTriangle(
        CreateVertex(v1),
        CreateVertex(v2),
        CreateVertex(v3)
    );
  }

  public void AddQuadAutoUV(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
    AddQuad(
        CreateVertex(v1),
        CreateVertex(v2),
        CreateVertex(v3),
        CreateVertex(v4)
    );
  }

  private MeshVertex CreateVertex(Vector3 position) {
    return new MeshVertex(
        position,
        Vector3.Up,
        CalculateAutoUV(position),
        new Plane(1, 0, 0, 1)
    );
  }

  private Vector2 CalculateAutoUV(Vector3 position) {
    return new Vector2(
        (position.X + _uvOffset.X) * _uvScale,
        (position.Z + _uvOffset.Y) * _uvScale
    );
  }

  private void AddVertex(MeshVertex vertex) {
    _surfaceTool.SetNormal(vertex.Normal);
    _surfaceTool.SetTangent(vertex.Tangent);
    _surfaceTool.SetUV(vertex.UV);
    _surfaceTool.AddVertex(vertex.Position);
  }

  public void SetUVScale(float scale) => _uvScale = scale;
  public void SetUVOffset(Vector2 offset) => _uvOffset = offset;

  public void ResetConfig() {
    _uvScale = _currentConfig.DefaultUVScale;
    _uvOffset = _currentConfig.DefaultUVOffset;
  }
}
