namespace HOB;

using Godot;
using System;

public partial class FPSLabel : Label {
  public override void _Process(double delta) {
    Text = ((int)Engine.GetFramesPerSecond()).ToString();
  }
}
