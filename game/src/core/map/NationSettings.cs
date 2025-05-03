namespace HOB;

using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class NationSettings : Resource {
  [Export] public string Name { get; private set; } = " ";
  [Export] public Array<ObjectSetting> ObjectSettings { get; private set; } = new();

  public NationSettings() { }
  public NationSettings(string name) {
    Name = name;
  }
}
