namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class MissionWidget : ButtonWidget, IWidgetFactory<MissionWidget> {
  public static MissionWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bpnm84svvttf3").Instantiate<MissionWidget>();
  }

  public void BindTo(MissionData mission) {
    Button.Text = mission.MissionName;
  }
}
