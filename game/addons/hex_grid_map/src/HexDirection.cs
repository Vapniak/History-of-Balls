namespace HexGridMap;

public enum HexDirection {
  First,
  Second,
  Third,
  Fourth,
  Fifth,
  Sixth,
}

public static class HexDirectionExtensions {
  public static HexDirection Opposite(this HexDirection direction) => (int)direction < 3 ? (direction + 3) : (direction - 3);
  public static HexDirection Previous(this HexDirection direction) => direction == HexDirection.First ? HexDirection.Sixth : (direction - 1);
  public static HexDirection Next(this HexDirection direction) => direction == HexDirection.Sixth ? HexDirection.First : (direction + 1);
}
