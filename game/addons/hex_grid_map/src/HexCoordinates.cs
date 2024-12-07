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
}
