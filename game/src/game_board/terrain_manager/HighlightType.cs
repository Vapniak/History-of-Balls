namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class HighlightType : Resource {
  [Export] public Color Color { get; private set; } = Colors.Transparent;
}
