namespace HOB;

using Godot;
using System;

public partial class EntityUi3D : Node {
  [Export] private ColorRect ColorRect { get; set; }
  public void SetColor(Color color) {
    ColorRect.Color = color;
  }
}
