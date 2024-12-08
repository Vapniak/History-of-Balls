namespace HexGridMap;

using Godot;

public struct HexEdge {
  public Vector3 V1, V2, V3, V4, V5;

  public HexEdge(Vector3 corner1, Vector3 corner2) {
    V1 = corner1;
    V2 = corner1.Lerp(corner2, 0.25f);
    V3 = corner1.Lerp(corner2, 0.5f);
    V4 = corner1.Lerp(corner2, 0.75f);
    V5 = corner2;
  }
}
