namespace HOB;

using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Team : Resource {
  [Export] public string Name { get; private set; }
  // TODO: add flag or icon
  [Export] public Color Color { get; private set; }

  public IMatchController Leader { get; set; }
}
