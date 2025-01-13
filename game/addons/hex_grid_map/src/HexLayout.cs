namespace HexGridMap;

using Godot;

public enum OrientationType {
  FlatTop,
  PointyTop
}

[GlobalClass]
public partial class HexLayout : Resource {
  [Export] public float HexCellScale { get; private set; } = 1;
  [Export] public OrientationType OrientationType { get; private set; } = OrientationType.FlatTop;
  [Export] public Offset Offset { get; private set; } = Offset.Even;
  [Export] public OffsetType OffsetType { get; private set; } = OffsetType.QOffset;

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

  public static readonly HexOrientation ORIENTATION_POINTY = new(
    Mathf.Sqrt(3.0f),
    Mathf.Sqrt(3.0f) / 2.0f,
    0.0f,
    3.0f / 2.0f,

    Mathf.Sqrt(3.0f) / 3.0f,
    -1.0f / 3.0f,
    0.0f,
    2.0f / 3.0f,

    0.5f
  );
  public static readonly HexOrientation ORIENTATION_FLAT = new(
    3.0f / 2.0f,
    0.0f,
    Mathf.Sqrt(3.0f) / 2.0f,
    Mathf.Sqrt(3.0f),

    2.0f / 3.0f,
    0.0f,
    -1.0f / 3.0f,
    Mathf.Sqrt(3.0f) / 3.0f,

    0.0f
  );

  public HexLayout() {

  }

  public HexLayout(OrientationType orientationType, float hexCellScale) {
    OrientationType = orientationType;
    HexCellScale = hexCellScale;
  }

  public Vector2 HexToPoint(HexCoordinates hex) {
    var x = ((Orientation.F0 * hex.Q) + (Orientation.F1 * hex.R)) * HexCellScale;
    var y = ((Orientation.F2 * hex.Q) + (Orientation.F3 * hex.R)) * HexCellScale;
    return new(x, y);
  }

  public HexCoordinates PointToHex(Vector2 point) {
    Vector2 pointOnGrid = new(point.X / HexCellScale, point.Y / HexCellScale);
    var x = (Orientation.B0 * pointOnGrid.X) + (Orientation.B1 * pointOnGrid.Y);
    var y = (Orientation.B2 * pointOnGrid.X) + (Orientation.B3 * pointOnGrid.Y);

    return new HexFractionalCoordinates(x, y).HexRound();
  }

  public HexOffsetCoordinates HexToOffset(HexCoordinates hexCoordinates) {
    return OffsetType switch {
      OffsetType.ROffset => hexCoordinates.Roffset(Offset),
      OffsetType.QOffset => hexCoordinates.Qoffset(Offset),
      _ => new(),// TODO: add null
    };
  }

  public HexCoordinates OffsetToHex(HexOffsetCoordinates offsetCoordinates) {
    return OffsetType switch {
      OffsetType.ROffset => offsetCoordinates.RoffsetToCube(Offset),
      OffsetType.QOffset => offsetCoordinates.QoffsetToCube(Offset),
      _ => new(),
    };
  }
}
