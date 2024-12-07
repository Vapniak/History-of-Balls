namespace HexGridMap;


public enum HexEdgeType {
  /// <summary>
  /// Elevation difference is zero.
  /// </summary>
  Flat,
  /// <summary>
  /// Elevation difference is one step
  /// </summary>
  Slope,
  /// <summary>
  /// Elevation difference is at least two steps.
  /// </summary>
  Cliff
}
