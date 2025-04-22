namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class LinkButton : Button {
  [Export] public string Uri { get; private set; } = "";

  public override void _Pressed() {
    OS.ShellOpen(Uri);
  }
}
