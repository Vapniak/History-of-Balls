namespace HexGridMap;

using System;
using Godot;

[GlobalClass]
public abstract partial class GridShape : Resource {

  public GridShape() { }

  public abstract HexCell[] CreateCells(HexGrid grid);

  /// <summary>
  /// Size of rect which fits whole map inside it.
  /// </summary>
  /// <returns></returns>
  public abstract Vector2I GetRectSize();

}
