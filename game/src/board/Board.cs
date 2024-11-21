namespace HOB;

using Godot;
using HexGridMap;

[GlobalClass]
public partial class Board : Node3D {
  [Export] private Mesh _hexMesh;
  [Export] public HexGridMap HexGridMap { get; private set; }

  public override void _Ready() {
    HexGridMap.BuildMap();

    MultiMesh multiMesh = new();
    multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
    multiMesh.InstanceCount = 1000;
    multiMesh.VisibleInstanceCount = 1000;
    multiMesh.Mesh = _hexMesh;

    var enumerator = HexGridMap.GetMapEnumerator();
    int count = 0;
    while (enumerator.MoveNext()) {
      var hex = enumerator.Current;
      var pos = HexGridMap.Layout.HexToPoint(hex);

      Transform3D transform = new();
      transform.Origin = new(pos.X, hex.Height, pos.Y);

      multiMesh.SetInstanceTransform(count, transform);
    }

    MultiMeshInstance3D instance = new();
    instance.Multimesh = multiMesh;
    AddChild(instance);
  }
}
