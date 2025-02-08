namespace HOB;

using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class MapSettings : Resource {
  [Export] public Array<CellDefinition> CellDefinitions;
}
