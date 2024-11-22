namespace HOB;

using Godot;
using HexGridMap;

[GlobalClass]
public partial class Board : Node3D {
  [Export] private Mesh _hexMesh;
  [Export] public HexGridMap HexGridMap { get; private set; }

  public override void _Ready() {
    HexGridMap.BuildMap();

    MultiMeshInstance3D multiMeshInstance = new() {
      TopLevel = true,
    };

    MultiMesh multiMesh = new() {
      TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
      InstanceCount = HexGridMap.Size(),
      Mesh = _hexMesh
    };

    AddChild(multiMeshInstance);

    var enumerator = HexGridMap.GetMapEnumerator();
    var count = 0;
    while (count < multiMesh.InstanceCount && enumerator.MoveNext()) {
      var hex = enumerator.Current;
      var pos = HexGridMap.Layout.HexToPoint(hex);

      Transform3D transform = new() {
        Origin = new(pos.X, hex.Height, pos.Y),
        Basis = Basis.Identity
      };

      multiMesh.SetInstanceTransform(count, transform);

      count++;
    }

    multiMeshInstance.Multimesh = multiMesh;
  }
}
