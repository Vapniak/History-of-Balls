namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class Hex : RefCounted {
  public readonly Vector3 CubeCoords;

  public readonly float Height;
  public Hex(Vector3 cubeCoords, float height = 0) {
    CubeCoords = cubeCoords;
    Height = height;

    if ((cubeCoords.X + cubeCoords.Y + cubeCoords.Z) != 0) {
      GD.PrintErr("q + r + s must be 0");
    }
  }

  public static Hex operator +(Hex cell, Hex other) => new(cell.CubeCoords + other.CubeCoords);
  public static Hex operator -(Hex cell, Hex other) => new(cell.CubeCoords - other.CubeCoords);
  public static Hex operator *(Hex cell, int scale) => new(cell.CubeCoords * scale);

  public int Length() => (int)(CubeCoords.Length() / 2);
  public int Distance(Hex to) => (to - this).Length();
}
