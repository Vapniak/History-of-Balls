namespace HexGridMap;

using Godot;
using System;

public partial class HexCoordinates : Resource {
  [field: Export]
  public int X { get; }
  [field: Export]
  public int Z { get; }

  public int Y => -X - Z;

  public float HexX => X + (Z / 2) + ((Z & 1) == 0 ? 0f : 0.5f);

  public float HexZ => Z * HexUtils.OUTER_TO_INNER;

  public HexCoordinates() { }
  public HexCoordinates(int x, int z) {
    X = x;
    Z = z;
  }

  public static HexCoordinates FromOffsetCoordinates(int x, int z) => new(x - (z / 2), z);

  public static HexCoordinates FromPosition(Vector3 position) {
    var x = position.X / HexUtils.INNER_DIAMETER;
    var y = -x;

    var offset = position.Z / (HexUtils.OUTER_RADIUS * 3f);
    x -= offset;
    y -= offset;

    var iX = Mathf.RoundToInt(x);
    var iY = Mathf.RoundToInt(y);
    var iZ = Mathf.RoundToInt(-x - y);

    if (iX + iY + iZ != 0) {
      var dX = Mathf.Abs(x - iX);
      var dY = Mathf.Abs(y - iY);
      var dZ = Mathf.Abs(-x - y - iZ);

      if (dX > dY && dX > dZ) {
        iX = -iY - iZ;
      }
      else if (dZ > dY) {
        iZ = -iX - iY;
      }
    }

    return new HexCoordinates(iX, iZ);
  }
}
