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
    var qi = Mathf.RoundToInt(Q);
    var ri = Mathf.RoundToInt(R);
    var si = Mathf.RoundToInt(S);
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
