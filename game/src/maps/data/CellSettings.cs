namespace HOB;

using Godot;
using System;

[GlobalClass, Tool]
public partial class CellSettings : Resource {
  [Export] public string Name { get; set; }
  [Export] public Color Color { get; set; }
  [Export] public int MoveCost { get; set; }
  [Export] public int Elevation { get; set; }
}
