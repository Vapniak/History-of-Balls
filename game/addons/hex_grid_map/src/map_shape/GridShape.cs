namespace HexGridMap;

using Godot;

[GlobalClass]
public abstract partial class GridShape : Resource {
  public GridShape() { }

  public abstract void CreateGrid(HexGrid grid);
  public abstract Vector2 GetSize();
}
