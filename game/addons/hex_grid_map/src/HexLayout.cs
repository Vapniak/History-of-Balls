namespace HexGridMap;

using System.Collections.Generic;
using Godot;

public enum OrientationType {
  FlatTop,
  PointyTop
}

[GlobalClass]
public partial class HexLayout : Resource {
  [Export] public float HexCellSize { get; private set; } = 1;
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
    HexCellSize = hexCellScale;
  }

  public Vector2 CubeToPoint(CubeCoord coord) {
    var x = ((Orientation.F0 * coord.Q) + (Orientation.F1 * coord.R)) * HexCellSize;
    var y = ((Orientation.F2 * coord.Q) + (Orientation.F3 * coord.R)) * HexCellSize;
    return new(x, y);
  }

  public CubeCoord PointToCube(Vector2 point) {
    Vector2 pointOnGrid = new(point.X / HexCellSize, point.Y / HexCellSize);
    var x = (Orientation.B0 * pointOnGrid.X) + (Orientation.B1 * pointOnGrid.Y);
    var y = (Orientation.B2 * pointOnGrid.X) + (Orientation.B3 * pointOnGrid.Y);

    return new CubeCoordF(x, y).Round();
  }

  public OffsetCoord CubeToOffset(CubeCoord coord) {
    return OffsetType switch {
      OffsetType.ROffset => coord.Roffset(Offset),
      OffsetType.QOffset => coord.Qoffset(Offset),
      _ => new(),// TODO: add null
    };
  }

  public CubeCoord OffsetToCube(OffsetCoord offsetCoord) {
    return OffsetType switch {
      OffsetType.ROffset => offsetCoord.RoffsetToCube(Offset),
      OffsetType.QOffset => offsetCoord.QoffsetToCube(Offset),
      _ => new(),
    };
  }

  public CubeCoord[] CoordsInRange(CubeCoord center, uint range) {
    var coords = new List<CubeCoord>();

    var rg = (int)range;
    for (var q = -rg; q <= rg; q++) {
      for (var r = Mathf.Max(-rg, -q - rg); r <= Mathf.Min(rg, -q + rg); r++) {
        coords.Add(center.Add(new(q, r)));
      }
    }

    return coords.ToArray();
  }

  public CubeCoord[] CoordsInLine(CubeCoord from, CubeCoord to) {
    var dist = from.Distance(to);
    List<CubeCoord> cells = new();

    for (var i = 0; i <= dist; i++) {
      cells.Add(from.Lerp(to, 1f / dist * i));
    }

    return cells.ToArray();
  }

  public CubeCoord[] CoordsInRing(CubeCoord center, uint radius) {
    List<CubeCoord> cells = new();

    var cell = center.Add(CubeCoord.GetDirection((HexDirection)4).Multiply((int)radius));

    for (var dir = HexDirection.Min; dir < HexDirection.Max; dir++) {
      for (var i = 0; i < radius; i++) {
        cells.Add(cell);
        cell = cell.GetNeighbor(dir);
      }
    }

    return cells.ToArray();
  }

  public Vector2 GetRealHexSize() {
    // TODO: fix the sizes to be correct
    var size = OrientationType switch {
      OrientationType.FlatTop => new Vector2(2f, Mathf.Sqrt(3)),
      OrientationType.PointyTop => new Vector2(Mathf.Sqrt(3), 2),
      _ => new()
    } * HexCellSize;
    return size;
  }

  public Vector2 GetSpacingBetweenHexes() {
    return new Vector2(Orientation.F0, Orientation.F3) * HexCellSize;
  }

  public Vector2 GetCorner(HexDirection index) {
    var angle = Mathf.Pi / 180f * 60 * (int)index;
    return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * HexCellSize;
  }
}
