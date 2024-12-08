namespace HexGridMap;

using Godot;

/// <summary>
/// Utilities for hex map.
/// </summary>
public static class HexUtils {
  public const float OUTER_TO_INNER = 0.866025404f;

  public const float INNER_TO_OUTER = 1f / OUTER_TO_INNER;

  public const float OUTER_RADIUS = 10f;

  public const float INNER_RADIUS = OUTER_RADIUS * OUTER_TO_INNER;

  public const float INNER_DIAMETER = INNER_RADIUS * 2f;

  public const int CHUNK_SIZE_X = 16;
  public const int CHUNK_SIZE_Z = 16;

  public const float SOLID_FACTOR = 0.8f;

  public const float CELL_PERTURB_STRENGTH = 4f;

  public static NoiseTexture2D NOISE_SOURCE = new();
  public const float NOISE_SCALE = 0.003f;

  public static readonly Vector3[] CORNERS = {
    new (0f, 0f, OUTER_RADIUS),
    new (INNER_RADIUS, 0f, 0.5f * OUTER_RADIUS),
    new (INNER_RADIUS, 0f, -0.5f * OUTER_RADIUS),
    new (0f, 0f, -OUTER_RADIUS),
    new (-INNER_RADIUS, 0f, -0.5f * OUTER_RADIUS),
    new (-INNER_RADIUS, 0f, 0.5f * OUTER_RADIUS),
    new (0f, 0f, OUTER_RADIUS)
  };

  public static float SampleNoise(Vector3 position) {
    var sample = NOISE_SOURCE.Noise.GetNoise2D((int)(position.X * NOISE_SCALE), (int)(position.Z * NOISE_SCALE));

    return sample;
  }
  public static Vector3 Perturb(Vector3 position) {
    var sample = SampleNoise(position);
    position.X += ((sample * 2f) - 1f) * CELL_PERTURB_STRENGTH;
    position.Z += ((sample * 2f) - 1f) * CELL_PERTURB_STRENGTH;
    return position;
  }

  public static Vector3 GetFirstSolidCorner(HexDirection direction) => CORNERS[(int)direction] * SOLID_FACTOR;
  public static Vector3 GetSecondSolidCorner(HexDirection direction) => CORNERS[(int)direction + 1] * SOLID_FACTOR;
}
