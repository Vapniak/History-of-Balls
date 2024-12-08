namespace HexGridMap;

using System;
using Godot;

/// <summary>
/// Utilities for hex map.
/// </summary>
public static class HexUtils {
  // TODO: change this to some grid map settings
  public const float OUTER_TO_INNER = 0.866025404f;

  public const float INNER_TO_OUTER = 1f / OUTER_TO_INNER;

  public const float OUTER_RADIUS = 10f;

  public const float INNER_RADIUS = OUTER_RADIUS * OUTER_TO_INNER;

  public const float INNER_DIAMETER = INNER_RADIUS * 2f;

  public const int CHUNK_SIZE_X = 16;
  public const int CHUNK_SIZE_Z = 16;

  public const float SOLID_FACTOR = 0.8f;

  private static readonly Vector3[] _corners = {
    new (0f, 0f, OUTER_RADIUS),
    new (INNER_RADIUS, 0f, 0.5f * OUTER_RADIUS),
    new (INNER_RADIUS, 0f, -0.5f * OUTER_RADIUS),
    new (0f, 0f, -OUTER_RADIUS),
    new (-INNER_RADIUS, 0f, -0.5f * OUTER_RADIUS),
    new (-INNER_RADIUS, 0f, 0.5f * OUTER_RADIUS),
  };

  public static Vector3 GetCorner(HexDirection direction) {

    return _corners[(int)direction];
  }
  public static Vector3 GetSolidCorner(HexDirection direction) => GetCorner(direction) * SOLID_FACTOR;
}
