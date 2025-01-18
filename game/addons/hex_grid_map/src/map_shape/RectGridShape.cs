namespace HexGridMap;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class RectGridShape : GridShape {
  [Export] public Vector2I Size { get; private set; } = new(10, 10);

  public override HexCell[] CreateCells(HexGrid grid) {
    List<HexCell> cells = new();
    for (var x = 0; x < Size.X; x++) {
      for (var y = 0; y < Size.Y; y++) {
        cells.Add(new(new OffsetCoord(x, y), grid));
      }
    }

    return cells.ToArray();
  }

  public override Vector2I GetRectSize() => Size;
}
