namespace HexGridMap;

using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class GridShape : Resource {
  private HexCoordinates[] HexGrid { get; set; }
  public GridShape() { }

  public HexCoordinates[] GetGrid() {
    HexGrid ??= CreateGrid();

    return HexGrid;
  }

  /// <summary>
  /// Size of rect which fill fit the map inside it.
  /// </summary>
  /// <returns></returns>
  public abstract Vector2I GetRectSize();

  protected abstract HexCoordinates[] CreateGrid();
}
