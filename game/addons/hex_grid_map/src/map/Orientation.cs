namespace HexGridMap;

using Godot;

public partial class Orientation : Resource {
  [Export] public float F0 { get; private set; }
  [Export] public float F1 { get; private set; }
  [Export] public float F2 { get; private set; }
  [Export] public float F3 { get; private set; }
  [Export] public float B0 { get; private set; }
  [Export] public float B1 { get; private set; }
  [Export] public float B2 { get; private set; }
  [Export] public float B3 { get; private set; }
  [Export] public float StartAngle { get; private set; }
  public Orientation(float f0, float f1, float f2, float f3, float b0, float b1, float b2, float b3, float startAngle) {
    F0 = f0;
    F1 = f1;
    F2 = f2;
    F3 = f3;
    B0 = b0;
    B1 = b1;
    B2 = b2;
    B3 = b3;
    StartAngle = startAngle;
  }
}
