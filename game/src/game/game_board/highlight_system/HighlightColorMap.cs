namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class HighlightColorMap : Resource {
  [Export] public HighlightType Type { get; set; }
  [Export] public Color Color { get; set; }
}
