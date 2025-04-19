namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class MissionWidget : Widget, IWidgetFactory<MissionWidget> {
  [Signal] public delegate void MissionSelectedEventHandler();
  [Export] private Button Button { get; set; } = default!;

  public static MissionWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bpnm84svvttf3").Instantiate<MissionWidget>();
  }

  public override void _Ready() {
    Button.Pressed += () => EmitSignal(SignalName.MissionSelected);
  }

  public void BindTo(MissionData mission) {
    Button.Text = mission.MissionName;
  }
}
