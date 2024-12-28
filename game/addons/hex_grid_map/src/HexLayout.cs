namespace HexGridMap;

using Godot;

public enum OrientationType {
  FlatTop,
  PointyTop
}

[GlobalClass]
public partial class HexLayout : Resource {
  [Export] public float HexCellScale { get; private set; } = 1;
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

  public HexLayout(OrientationType orientationType, float hexCellScale) {
    OrientationType = orientationType;
    HexCellScale = hexCellScale;
  }

  public Vector2 HexCoordinatesToPoint(HexCoordinates coordinates) {
    var x = ((Orientation.F0 * coordinates.Q) + (Orientation.F1 * coordinates.R)) * HexCellScale;
    var y = ((Orientation.F2 * coordinates.Q) + (Orientation.F3 * coordinates.R)) * HexCellScale;
    return new(x, y);
  }

  public HexCoordinates PointToHexCoordinates(Vector2 point) {
    Vector2 pointOnGrid = new(point.X / HexCellScale, point.Y / HexCellScale);
    var x = (Orientation.B0 * pointOnGrid.X) + (Orientation.B1 * pointOnGrid.Y);
    var y = (Orientation.B2 * pointOnGrid.X) + (Orientation.B3 * pointOnGrid.Y);

    return new HexFractionalCoordinates(x, y).HexRound();
  }
}
