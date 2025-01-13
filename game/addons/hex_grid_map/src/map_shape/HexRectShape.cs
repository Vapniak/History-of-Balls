namespace HexGridMap;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class HexRectShape : GridShape {
  [Export] public Vector2I Size { get; private set; } = new(10, 10);
  protected override HexCoordinates[] CreateGrid(HexLayout layout) {
    List<HexCoordinates> grid = new();
    for (var x = 0; x < Size.X; x++) {
      for (var y = 0; y < Size.Y; y++) {
        grid.Add(layout.OffsetToHex(new HexOffsetCoordinates(x, y)));
      }
    }

    return grid.ToArray();
  }

  public override Vector2I GetRectSize() => Size;
}
