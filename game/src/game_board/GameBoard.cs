namespace HOB;

using Godot;
using HexGridMap;
using System;

public partial class GameBoard : Node3D {

  [Export] public HexGrid Grid { get; private set; }

  [Export] private PackedScene _debugMesh;

  public override void _Ready() {
    Grid.CreateGrid();

    var enumerator = Grid.GetGridEnumerator();
    while (enumerator.MoveNext()) {
      var mesh = _debugMesh.Instantiate<Node3D>();

      var hex = enumerator.Current;
      var point = Grid.Layout.HexToPoint(hex);

      mesh.Position = new(point.X, 0, point.Y);
      AddChild(mesh);
    }
  }
}
