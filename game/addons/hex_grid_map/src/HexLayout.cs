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

  public Vector2 HexToPoint(CubeCoord hex) {
    var x = ((Orientation.F0 * hex.Q) + (Orientation.F1 * hex.R)) * HexCellSize;
    var y = ((Orientation.F2 * hex.Q) + (Orientation.F3 * hex.R)) * HexCellSize;
    return new(x, y);
  }

  public CubeCoord PointToHex(Vector2 point) {
    Vector2 pointOnGrid = new(point.X / HexCellSize, point.Y / HexCellSize);
    var x = (Orientation.B0 * pointOnGrid.X) + (Orientation.B1 * pointOnGrid.Y);
    var y = (Orientation.B2 * pointOnGrid.X) + (Orientation.B3 * pointOnGrid.Y);

    return new CubeCoordF(x, y).Round();
  }

  public OffsetCoord HexToOffset(CubeCoord hexCoord) {
    return OffsetType switch {
      OffsetType.ROffset => hexCoord.Roffset(Offset),
      OffsetType.QOffset => hexCoord.Qoffset(Offset),
      _ => new(),// TODO: add null
    };
  }

  public CubeCoord OffsetToHex(OffsetCoord offsetCoord) {
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
}
