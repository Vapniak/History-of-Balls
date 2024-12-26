namespace HexGridMap;

using Godot;

public enum OrientationType {
  FlatTop,
  PointyTop
}

[GlobalClass]
public partial class HexLayout : Resource {
  [Export] public Vector2 Scale { get; private set; } = Vector2.One;
  [Export] public Vector2 Origin { get; private set; }
  [Export] public OrientationType OrientationType = OrientationType.FlatTop;

  public HexOrientation Orientation {
    get {
      if (OrientationType == OrientationType.FlatTop) {
        return ORIENTATION_FLAT;
      }
      else {
        return ORIENTATION_POINTY;
      }
    }
  }

  // MAGIC NUMBERS I DON'T KNOW WHERE THEY COME FROM
  public static readonly HexOrientation ORIENTATION_POINTY = new(Mathf.Sqrt(3.0f), Mathf.Sqrt(3.0f) / 2.0f, 0.0f, 3.0f / 2.0f, Mathf.Sqrt(3.0f) / 3.0f, -1.0f / 3.0f, 0.0f, 2.0f / 3.0f, 0.5f);
  public static readonly HexOrientation ORIENTATION_FLAT = new(3.0f / 2.0f, 0.0f, Mathf.Sqrt(3.0f) / 2.0f, Mathf.Sqrt(3.0f), 2.0f / 3.0f, 0.0f, -1.0f / 3.0f, Mathf.Sqrt(3.0f) / 3.0f, 0.0f);

  public HexLayout() {

  }

  public HexLayout(OrientationType orientationType, Vector2 size, Vector2 origin) {
    OrientationType = orientationType;
    Scale = size;
    Origin = origin;
  }

  public Vector2 HexToPoint(HexCell hex) {
    var x = ((Orientation.F0 * hex.Coordinates.Q) + (Orientation.F1 * hex.Coordinates.R)) * Scale.X;
    var y = ((Orientation.F2 * hex.Coordinates.Q) + (Orientation.F3 * hex.Coordinates.R)) * Scale.Y;
    return new(x + Origin.X, y + Origin.Y);
  }

  public HexCell PointToHex(Vector2 point) {
    Vector2 pointOnGrid = new((point.X - Origin.X) / Scale.X, (point.Y - Origin.Y) / Scale.Y);
    var x = (Orientation.B0 * pointOnGrid.X) + (Orientation.B1 * pointOnGrid.Y);
    var y = (Orientation.B2 * pointOnGrid.X) + (Orientation.B3 * pointOnGrid.Y);

    return new(new(x, y));
  }
}
