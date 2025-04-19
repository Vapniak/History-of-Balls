namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class CurrentTimeWidget : HOBWidget {
  [Export] private Label CurrentTimeLabel { get; set; } = default!;
  public override void _Ready() {
    var timer = new Timer() {
      WaitTime = 1,
    };
    timer.Timeout += UpdateTime;
    AddChild(timer);
    timer.Start();
  }

  private void UpdateTime() {
    var now = DateTime.Now;

    var timeString = now.ToString("HH:mm");

    CurrentTimeLabel.Text = timeString;
  }
}
