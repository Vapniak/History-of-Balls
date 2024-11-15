namespace HexGridMap;

using System.Collections.Generic;
using Godot;

// https://www.redblobgames.com/grids/hexagons/implementation.html


[GlobalClass]
public partial class HexGridMap : Node3D {
  // TEMPORARY
  [Export] private Mesh _mesh;
  [Export] public Layout Layout { get; private set; }
  [Export] public MapShape MapShape { get; private set; }

  public HashSet<Hex> Map = new();

  public override void _Ready() {
    MapShape.BuildMap(this);

    // visualization
    foreach (var hex in Map) {
      MeshInstance3D mesh = new() {
        Mesh = _mesh
      };

      var pixel = Layout.HexToPoint(hex);
      mesh.Position = new(pixel.X, 0, pixel.Y);
      GD.Print(pixel);

      AddChild(mesh);
    }
  }
}
