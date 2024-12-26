namespace HexGridMap;

using Godot;

public struct HexCoordinates {
  public float Q { get; private set; }
  public float R { get; private set; }
  public readonly float S => -Q - R;
  public HexCoordinates(float q, float r) {
    Q = q;
    R = r;
  }

  private static readonly HexCoordinates[] _directions = {
    new(1, 0),
    new(1, -1),
    new(0, -1),
    new(-1, 0),
    new(-1, 1),
    new(0, 1)
  };

  public static HexCoordinates operator +(HexCoordinates hex, HexCoordinates other) => new(hex.Q + other.Q, hex.R + other.R);
  public static HexCoordinates operator -(HexCoordinates hex, HexCoordinates other) => new(hex.Q - other.Q, hex.R - other.R);
  public static HexCoordinates operator *(HexCoordinates hex, float scale) => new(hex.Q * scale, hex.R * scale);

  public static HexCoordinates Direction(HexDirection direction) {
    return _directions[(int)direction];
  }

  public readonly float Length() {
    return (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;
  }

  public readonly float Distance(HexCoordinates other) {
    return (this - other).Length();
  }
}
