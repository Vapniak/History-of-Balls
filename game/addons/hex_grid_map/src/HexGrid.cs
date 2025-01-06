namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class HexGrid : Node {
  [Export] private GridShape GridShape { get; set; }
  [Export] private HexLayout Layout { get; set; } = new();

  public HexCoordinates[] CreateGridShape() {
    return GridShape.CreateGridShape();
  }

  public HexLayout GetLayout() => Layout;
}
