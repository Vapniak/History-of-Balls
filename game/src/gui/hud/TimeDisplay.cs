namespace HOB;

using GameplayFramework;
using Godot;
using System;

public partial class TimeDisplay : Control {
  [Export] private Timer _timer = default!;
  [Export] private Label CurrentTimeLabel { get; set; } = default!;
  [Export] private Label TimePlayedLabel { get; set; } = default!;
  [Export] private Button TimeScaleButton { get; set; } = default!;

  public override void _Ready() {
    _timer.Timeout += UpdateTime;
    TimeScaleButton.Pressed += () => {
      Engine.TimeScale = Engine.TimeScale == 2 ? 1 : 2;
    };
  }

  public override void _Process(double delta) {
    TimeScaleButton.Text = $"{Engine.TimeScale}x";
  }

  private void UpdateTime() {
    var now = DateTime.Now;

    var timeString = now.ToString("HH:mm");

    CurrentTimeLabel.Text = timeString;
    var time = TimeSpan.FromMilliseconds(GameInstance.GetGameState<IMatchGameState>().GameTimeMSec);
    TimePlayedLabel.Text = $"{time.Minutes:00}:{time.Seconds:00}";
  }
}
