namespace HOB;

using Godot;
using System;

[GlobalClass, Tool]
public partial class CellDefinition : Resource {
  [Export] public string Name { get; set; }
  [Export] public Color Color { get; set; }
  [Export] public int MoveCost { get; set; }
}
