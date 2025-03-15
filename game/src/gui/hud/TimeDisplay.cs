namespace HOB;

using GameplayFramework;
using Godot;
using System;

public partial class TimeDisplay : Control {
  [Export] private Timer _timer;
  [Export] private Label CurrentTimeLabel { get; set; }
  [Export] private Label TimePlayedLabel { get; set; }

  public override void _Ready() {
    _timer.Timeout += UpdateTime;
  }

  private void UpdateTime() {
    var now = DateTime.Now;

    var timeString = now.ToString("HH:mm");

    CurrentTimeLabel.Text = timeString;
    var time = TimeSpan.FromMilliseconds(GameInstance.GetGameState<IMatchGameState>().GameTimeMSec);
    TimePlayedLabel.Text = $"{time.Minutes:00}:{time.Seconds:00}";
  }
}
