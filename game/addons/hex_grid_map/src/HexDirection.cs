namespace HexGridMap;

public enum HexDirection {
  Min,
  Max = 6
}

public static class HexDirectionExtensions {
  public static HexDirection Opposite(this HexDirection direction) => (int)direction < 3 ? (direction + 3) : (direction - 3);
  public static HexDirection Previous(this HexDirection direction) => direction == HexDirection.Min ? HexDirection.Max : (direction - 1);
  public static HexDirection Next(this HexDirection direction) => direction == HexDirection.Max ? HexDirection.Min : (direction + 1);
}
