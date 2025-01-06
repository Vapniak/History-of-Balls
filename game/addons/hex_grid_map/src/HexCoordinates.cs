namespace HexGridMap;

using Godot;

public struct HexCoordinates {
  public int Q { get; private set; }
  public int R { get; private set; }

  public readonly int S => -Q - R;

  public HexCoordinates(int q, int r) {
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

  public static HexCoordinates GetDirection(HexDirection direction) {
    return _directions[(int)direction];
  }

  public readonly HexCoordinates GetNeighbor(HexDirection direction) {
    return Add(GetDirection(direction));
  }

  public readonly int Length() {
    return (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;
  }

  public readonly int Distance(HexCoordinates other) {
    return Substract(other).Length();
  }

  public override readonly string ToString() {
    return "Q: " + Q + ", R: " + R + ", S: " + S;
  }


  public static bool operator ==(HexCoordinates lhs, HexCoordinates rhs) => lhs.Q == rhs.Q && lhs.R == rhs.R;
  public static bool operator !=(HexCoordinates lhs, HexCoordinates rhs) => lhs.Q != rhs.Q || lhs.R != rhs.R;
  public readonly HexCoordinates Add(HexCoordinates other) => new(Q + other.Q, R + other.R);
  public readonly HexCoordinates Substract(HexCoordinates other) => new(Q - other.Q, R - other.R);
  public readonly HexCoordinates Multiply(int scalar) => new(Q * scalar, R * scalar);
}
