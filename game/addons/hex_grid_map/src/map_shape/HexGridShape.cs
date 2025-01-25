namespace HexGridMap;

using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class HexGridShape : GridShape {
  [Export] public int Size { get; private set; } = 10;

  public HexGridShape() { }
  public override T[] CreateCells<T>(Func<CubeCoord, T> createCell, HexLayout layout) {
    List<T> cells = new();
    for (var q = -Size; q <= Size; q++) {
      var r1 = Mathf.Max(-Size, -q - Size);
      var r2 = Mathf.Min(Size, -q + Size);
      for (var r = r1; r <= r2; r++) {
        cells.Add(createCell(new CubeCoord(q, r)));
      }
    }

    return cells.ToArray();
  }

  public override Vector2I GetRectSize() => Vector2I.One * 2 * Size;
}
