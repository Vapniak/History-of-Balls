namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class CurrentVersionWidget : HOBWidget, IWidgetFactory<CurrentVersionWidget> {
  [Export] private Label VersionLabel { get; set; } = default!;
  public static CurrentVersionWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://hnn3v40nxtxw").Instantiate<CurrentVersionWidget>();
  }

  public override void _Ready() {
    if (OS.HasFeature("movie")) {
      Hide();
    }
    VersionLabel.Text = ProjectSettings.GetSetting("application/config/version", "1.0").AsString();
  }
}
