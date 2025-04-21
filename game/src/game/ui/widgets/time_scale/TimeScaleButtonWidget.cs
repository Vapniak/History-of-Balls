namespace HOB;

using Godot;
using System;
using System.Linq;
using WidgetSystem;

[GlobalClass]
public partial class TimeScaleButtonWidget : HOBWidget {
  [Signal] public delegate void TimeScaleButtonPressedEventHandler();

  [Export] private ButtonWidget ButtonWidget { get; set; } = default!;
  public override void _Process(double delta) {
    ButtonWidget.Configure(label => label.Text =
      string.Concat(Enumerable.Repeat(">", (int)Engine.TimeScale)));
  }
  private void OnTimeScaleButtonPressed() {
    EmitSignal(SignalName.TimeScaleButtonPressed);
  }
}
