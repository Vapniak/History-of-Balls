namespace HOB;

using Godot;
using System;

[GlobalClass, Tool]
public partial class HexSetting : Resource {
  [Export] public int MoveCost { get; set; } = 1;
  [Export] public Color Visualizaiton { get; set; }
}
