namespace HOB;

using Godot;
using System;

public partial class EntityUi3D : Node {
  [Export] private Label NameLabel { get; set; }

  public void SetNameLabel(string name) {
    NameLabel.Text = name;
  }
}
