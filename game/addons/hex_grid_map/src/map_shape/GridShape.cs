namespace HexGridMap;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class GridShape : Resource {
  public GridShape() { }

  public abstract HexCoordinates[] CreateGridShape();

  /// <summary>
  /// Size of rect which fill fit the map inside it.
  /// </summary>
  /// <returns></returns>
  public abstract Vector2I GetRectSize();
}
