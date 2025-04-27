namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class MissionWidget : HOBWidget, IWidgetFactory<MissionWidget> {
  [Export] public ButtonWidget ButtonWidget { get; private set; } = default!;

  [Export] private Label MissionNameLabel { get; set; } = default!;
  public static MissionWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bpnm84svvttf3").Instantiate<MissionWidget>();
  }

  public void BindTo(MissionData mission) {
    MissionNameLabel.Text
     = mission.MissionName;
  }
}
