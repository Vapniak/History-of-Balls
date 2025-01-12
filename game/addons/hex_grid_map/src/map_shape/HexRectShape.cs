namespace HexGridMap;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class HexRectShape : GridShape {
  [Export] public Vector2I Size { get; private set; } = new(10, 10);
  [Export] public Offset Offset;
  [Export] public OffsetType OffsetType;
  protected override HexCoordinates[] CreateGrid() {
    List<HexCoordinates> grid = new();
    for (var x = 0; x < Size.X; x++) {
      for (var y = 0; y < Size.Y; y++) {
        if (OffsetType == OffsetType.ROffset) {
          grid.Add(new HexOffsetCoordinates(x, y).RoffsetToCube(Offset));
        }
        else if (OffsetType == OffsetType.QOffset) {
          grid.Add(new HexOffsetCoordinates(x, y).QoffsetToCube(Offset));
        }
      }
    }

    return grid.ToArray();
  }

  public override Vector2I GetRectSize() => Size;
}
