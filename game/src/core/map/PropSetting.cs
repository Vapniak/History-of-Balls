namespace HOB;

using Godot;

[GlobalClass, Tool]
public partial class PropSetting : Resource {
  [Export] public PackedScene PropScene { get; private set; } = default!;
  [Export(PropertyHint.Range, "0,100")] public float Chance { get; private set; }
  [Export] public Vector2I AmountRange { get; private set; } = new(1, 1);
  [Export] public Vector2 ScaleRange { get; private set; } = new(0.9f, 1.1f);

  public (Mesh mesh, Transform3D transform) GetMesh() {
    var node = PropScene.InstantiateOrNull<Node3D>();
    var meshInstance = node.GetChildByType<MeshInstance3D>();
    node.QueueFree();
    return (meshInstance.Mesh, meshInstance.Transform);
  }
}