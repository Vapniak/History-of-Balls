namespace HOB;

using Godot;
using HexGridMap;

[GlobalClass]
public partial class Board : Node3D {
  [Export] private Mesh _hexMesh;
  [Export] public HexGridMap HexGridMap { get; private set; }
  [Export] private float _elevationStep = 1;

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

      var height = (_elevationStep * hex.Elevation) + 0.1f;

      Transform3D transform = new() {
        Origin = new(pos.X, 0, pos.Y),
        Basis = Basis.Identity.Scaled(new(HexGridMap.Layout.Size.X, height, HexGridMap.Layout.Size.Y))
      };

      // IT HAS TO BE THIS WAY, DON'T CHANGE PLEASEE :)
      transform.Origin += Vector3.Up * height / 4;

      multiMesh.SetInstanceTransform(count, transform);

      count++;
    }

    multiMeshInstance.Multimesh = multiMesh;
  }
}
