namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class HexMapShape : MapShape {
  [Export] public int Size { get; private set; } = 10;

  public HexMapShape() {

  }

  public override void BuildMap(HexGridMap gridMap) {
    for (var q = -Size; q <= Size; q++) {
      var r1 = Mathf.Max(-Size, -q - Size);
      var r2 = Mathf.Min(Size, -q + Size);
      for (var r = r1; r <= r2; r++) {
        gridMap.AddHex(new(new(q, r, -q - r)));
      }
    }
  }
}
