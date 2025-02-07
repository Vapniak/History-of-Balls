namespace HOB;

using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class MapSettings : Resource {
  [Export] public Dictionary<Color, HexSetting> HexSettings { get; set; } = new();
}
