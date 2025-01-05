namespace HexGridMap;

using Godot;

[GlobalClass]
public abstract partial class GridShape : Resource {
  public GridShape() { }

  public abstract void CreateGridShape(HexGrid grid);
}
