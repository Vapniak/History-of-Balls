namespace HOB;

using System;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class NationsSettings : Resource {
  [Export] public Array<NationSettings> Settings { get; private set; } = new();
}
