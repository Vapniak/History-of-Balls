namespace HOB;

using Godot;
using System;

public partial class VersionLabel : Label {
  public override void _Ready() {
    Text = ProjectSettings.GetSetting("application/config/version", "1.0").AsString();
  }
}
