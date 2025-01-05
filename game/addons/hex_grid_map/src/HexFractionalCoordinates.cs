namespace HexGridMap;

using Godot;

public struct HexFractionalCoordinates {
  public float Q { get; private set; }

  public float R { get; private set; }

  public readonly float S => -Q - R;

  public HexFractionalCoordinates(float q, float r) {
    Q = q;
    R = r;
  }

  public readonly HexCoordinates HexRound() {
    var qi = (int)Mathf.Round(Q);
    var ri = (int)Mathf.Round(R);
    var si = (int)Mathf.Round(S);
    double qDiff = Mathf.Abs(qi - Q);
    double rDiff = Mathf.Abs(ri - R);
    double sDiff = Mathf.Abs(si - S);
    if (qDiff > rDiff && qDiff > sDiff) {
      qi = -ri - si;
    }
    else
        if (rDiff > sDiff) {
      ri = -qi - si;
    }
    else {
      si = -qi - ri;
    }
    return new(qi, ri);
  }
  public readonly float Distance(HexFractionalCoordinates other) => Substract(other).Length();
  public readonly float Length() => (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;
  public readonly HexFractionalCoordinates Add(HexFractionalCoordinates other) => new(Q + other.Q, R + other.R);
  public readonly HexFractionalCoordinates Substract(HexFractionalCoordinates other) => new(Q - other.Q, R - other.R);
  public readonly HexFractionalCoordinates Multiply(float scalar) => new(Q * scalar, R * scalar);
}
