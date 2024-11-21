namespace HexGridMap;

using Godot;

[GlobalClass]
public abstract partial class MapShape : Resource {
  public MapShape() {

  }
  public abstract void BuildMap(HexGridMap gridMap);
}
