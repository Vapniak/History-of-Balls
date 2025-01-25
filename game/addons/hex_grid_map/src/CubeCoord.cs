namespace HexGridMap;

using Godot;

public struct CubeCoord {
  public int Q { get; private set; }
  public int R { get; private set; }

  public readonly int S => -Q - R;

  public CubeCoord(int q, int r) {
    Q = q;
    R = r;
  }

  private static readonly CubeCoord[] _directions = {
    new(1, 0),
    new(1, -1),
    new(0, -1),
    new(-1, 0),
    new(-1, 1),
    new(0, 1)
  };

  public static CubeCoord GetDirection(HexDirection direction) {
    return _directions[(int)direction];
  }

  public readonly CubeCoord GetNeighbor(HexDirection direction) {
    return Add(GetDirection(direction));
  }

  public readonly int Length() {
    return (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;
  }

  public readonly int Distance(CubeCoord other) {
    return Substract(other).Length();
  }

  public override readonly string ToString() {
    return "Q: " + Q + ", R: " + R + ", S: " + S;
  }


  public static bool operator ==(CubeCoord lhs, CubeCoord rhs) => lhs.Q == rhs.Q && lhs.R == rhs.R;
  public static bool operator !=(CubeCoord lhs, CubeCoord rhs) => lhs.Q != rhs.Q || lhs.R != rhs.R;
  public readonly CubeCoord Add(CubeCoord other) => new(Q + other.Q, R + other.R);
  public readonly CubeCoord Substract(CubeCoord other) => new(Q - other.Q, R - other.R);
  public readonly CubeCoord Multiply(int scalar) => new(Q * scalar, R * scalar);


  public readonly OffsetCoord Qoffset(Offset offset) {
    var col = Q;
    var row = R + ((Q + ((int)offset * (Q & 1))) / 2);
    return new OffsetCoord(col, row);
  }


  public readonly OffsetCoord Roffset(Offset offset) {
    var col = Q + ((R + ((int)offset * (R & 1))) / 2);
    var row = R;


    return new OffsetCoord(col, row);
  }
}
