namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class HexGridShape : GridShape {
  [Export] public int Size { get; private set; } = 10;

  public HexGridShape() { }
  public override void CreateGrid(HexGrid grid) {
    for (var q = -Size; q <= Size; q++) {
      var r1 = Mathf.Max(-Size, -q - Size);
      var r2 = Mathf.Min(Size, -q + Size);
      for (var r = r1; r <= r2; r++) {
        grid.AddHex(new(new(q, r)));
      }
    }
  }

  public override Vector2 GetSize() {
    return new(10, 10);
  }
}
