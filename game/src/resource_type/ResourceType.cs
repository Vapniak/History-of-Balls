namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class ResourceType : Resource {
  [Export] public string Name { get; private set; } = "Resource";
  [Export] public Texture2D Icon { get; private set; }

  [Notify]
  public uint Value {
    get => _value.Get();
    set => _value.Set(value);
  }
}
