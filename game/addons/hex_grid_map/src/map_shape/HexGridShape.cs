namespace HexGridMap;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class HexGridShape : GridShape {
  [Export] public int Size { get; private set; } = 10;

  public HexGridShape() { }
  protected override HexCoordinates[] CreateGrid() {
    List<HexCoordinates> grid = new();
    for (var q = -Size; q <= Size; q++) {
      var r1 = Mathf.Max(-Size, -q - Size);
      var r2 = Mathf.Min(Size, -q + Size);
      for (var r = r1; r <= r2; r++) {
        grid.Add(new(q, r));
      }
    }

    return grid.ToArray();
  }

  public override Vector2I GetRectSize() => Vector2I.One * 2 * Size;
}
