namespace HexGridMap;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class GridShape : Resource {
  public GridShape() { }

  public abstract HexCoordinates[] CreateGridShape();
}
