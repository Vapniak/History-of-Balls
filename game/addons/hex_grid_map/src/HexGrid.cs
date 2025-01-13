namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class HexGrid : Node {
  [Export] public GridShape GridShape { get; private set; }
  [Export] public HexLayout Layout { get; private set; } = new();

  public HexCoordinates[] GetHexGrid() {
    return GridShape.GetGrid(GetLayout());
  }

  public HexLayout GetLayout() => Layout;
  public GridShape GetGridShape() => GridShape;

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
