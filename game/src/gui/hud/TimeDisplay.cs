namespace HOB;

using Godot;
using System;

public partial class TimeDisplay : Label {
  [Export] private Timer _timer;

  public override void _Ready() {
    _timer.Timeout += UpdateTime;
  }

  private void UpdateTime() {
    var now = DateTime.Now;

    var timeString = now.ToString("HH:mm");

    Text = timeString;
  }
}
