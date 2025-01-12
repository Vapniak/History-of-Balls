namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class HexGrid : Node {
  [Export] private HexGridSettings GridSettings { get; set; }

  public HexCoordinates[] CreateGrid() {
    return GridSettings.GridShape.CreateGridShape();
  }

  public HexLayout GetLayout() => GridSettings.Layout;
  public GridShape GetGridShape() => GridSettings.GridShape;

  public Vector2I GetRectSize() {
    return GetGridShape().GetRectSize();
  }

  public Vector2 GetRealSize() {
    var size = new Vector2();

    var m = GetLayout().Orientation;
    // FIXME: fix the size, some misaligment is visible
    size.X = GetRectSize().X * GetLayout().HexCellScale * m.F0;
    size.Y = GetRectSize().Y * GetLayout().HexCellScale * m.F3;

    return size;
  }
}
