namespace HOB;

using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class CellSetting : Resource {
  [Export] public string Name { get; set; } = default!;
  [Export] public Color Color { get; set; }
  [Export] public int MoveCost { get; set; }
  [Export] public int Elevation { get; set; }
  [Export] public bool IsWater { get; set; } = false;
  [Export] public Array<PropSetting> Props { get; set; } = new();
}
