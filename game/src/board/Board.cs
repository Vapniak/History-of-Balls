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
        Origin = new(pos.X, 0, pos.Y),
        Basis = Basis.Identity
      };

      transform = transform.Scaled(new(1, hex.Height + .1f, 1));


      // IT HAS TO BE THIS WAY, DON'T CHANGE PLEASEE :)
      transform.Origin += Vector3.Up * hex.Height / 4;



      multiMesh.SetInstanceTransform(count, transform);

      count++;
    }

    multiMeshInstance.Multimesh = multiMesh;
  }
}
