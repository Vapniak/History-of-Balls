namespace HexGridMap;

using Godot;

public struct CubeCoordF {
  public float Q { get; private set; }

  public float R { get; private set; }

  public readonly float S => -Q - R;

  public CubeCoordF(float q, float r) {
    Q = q;
    R = r;
  }

  public CubeCoordF(CubeCoord coord) {
    Q = coord.Q;
    R = coord.R;
  }

  public readonly CubeCoord Round() {
    var q = Mathf.RoundToInt(Q);
    var r = Mathf.RoundToInt(R);
    var s = Mathf.RoundToInt(S);

    var qDiff = Mathf.Abs(q - Q);
    var rDiff = Mathf.Abs(r - R);
    var sDiff = Mathf.Abs(s - S);

    if (qDiff > rDiff && qDiff > sDiff) {
      q = -r - s;
    }
    else if (rDiff > sDiff) {
      r = -q - s;
    }
    else {
      s = -q - r;
    }

    return new CubeCoord(q, r);
  }

  public static CubeCoordF operator +(CubeCoordF a, CubeCoordF b) =>
      new(a.Q + b.Q, a.R + b.R);
  public readonly float Distance(CubeCoordF other) => Substract(other).Length();
  public readonly float Length() => (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;
  public readonly CubeCoordF Lerp(CubeCoordF to, double weight) {
    return new((float)Mathf.Lerp(Q, to.Q, weight), (float)Mathf.Lerp(R, to.R, weight));
  }
  public readonly CubeCoordF Add(CubeCoordF other) => new(Q + other.Q, R + other.R);
  public readonly CubeCoordF Substract(CubeCoordF other) => new(Q - other.Q, R - other.R);
  public readonly CubeCoordF Multiply(float scalar) => new(Q * scalar, R * scalar);

  public static implicit operator CubeCoord(CubeCoordF coord) => coord.Round();
}
