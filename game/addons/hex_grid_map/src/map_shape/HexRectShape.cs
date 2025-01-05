namespace HexGridMap;

using Godot;
using System;

[GlobalClass]
public partial class HexRectShape : GridShape {
  [Export] public Vector2 Size { get; private set; } = new(10, 10);
  [Export] public Offset Offset;
  [Export] public OffsetType OffsetType;
  public override void CreateGridShape(HexGrid grid) {
    for (var x = 0; x < Size.X; x++) {
      for (var y = 0; y < Size.Y; y++) {
        if (OffsetType == OffsetType.ROffset) {
          grid.AddHex(HexOffsetCoordinates.RoffsetToCube(Offset, new HexOffsetCoordinates(x, y)));
        }
        else if (OffsetType == OffsetType.QOffset) {
          grid.AddHex(HexOffsetCoordinates.QoffsetToCube(Offset, new HexOffsetCoordinates(x, y)));
        }
      }
    }
  }
}
