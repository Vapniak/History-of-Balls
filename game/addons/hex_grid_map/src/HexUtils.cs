namespace HexGridMap;

using Godot;

/// <summary>
/// Utilities for hex map.
/// </summary>
public static class HexUtils {
  public const float OUTER_TO_INNER = 866025404f;

  public const float INNER_TO_OUTER = 1f / OUTER_TO_INNER;

  public const float OUTER_RADIUS = 10f;

  public const float INNER_RADIUS = OUTER_RADIUS * OUTER_TO_INNER;

  public const float INNER_DIAMETER = INNER_RADIUS * 2f;

  public static readonly Vector3[] CORNERS = {
    new(0f, 0f, OUTER_RADIUS),
    new (INNER_RADIUS, 0f, 0.5f * OUTER_RADIUS),
    new (INNER_RADIUS, 0f, -0.5f * OUTER_RADIUS),
    new (0f, 0f, -OUTER_RADIUS),
    new (-INNER_RADIUS, 0f, -0.5f * OUTER_RADIUS),
    new (-INNER_RADIUS, 0f, 0.5f * OUTER_RADIUS)
  };
}
