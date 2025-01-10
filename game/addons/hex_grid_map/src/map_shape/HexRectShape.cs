namespace HexGridMap;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class HexRectShape : GridShape {
  [Export] public Vector2I Size { get; private set; } = new(10, 10);
  [Export] public Offset Offset;
  [Export] public OffsetType OffsetType;
  public override HexCoordinates[] CreateGridShape() {
    List<HexCoordinates> grid = new();
    for (var x = -Size.X / 2; x < Size.X / 2; x++) {
      for (var y = -Size.Y / 2; y < -Size.Y / 2; y++) {
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
