namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class TimeScaleButtonWidget : HOBWidget {
  [Signal] public delegate void TimeScaleButtonPressedEventHandler();

  [Export] private Button TimeScaleButton { get; set; } = default!;
  public override void _Process(double delta) {
    TimeScaleButton.Text = $"{Engine.TimeScale}x";
  }
  private void OnTimeScaleButtonPressed() {
    EmitSignal(SignalName.TimeScaleButtonPressed);
  }
}
