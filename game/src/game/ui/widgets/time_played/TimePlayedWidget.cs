namespace HOB;

using GameplayFramework;
using Godot;
using System;

[GlobalClass]
public partial class TimePlayedWidget : HOBWidget {
  [Export] private Label TimePlayedLabel { get; set; } = default!;

  public override void _Ready() {
    UpdateTime();

    var timer = new Timer() {
      WaitTime = 1,
    };
    timer.Timeout += UpdateTime;
    AddChild(timer);
    timer.Start();
  }

  private void UpdateTime() {
    if (GameInstance.GetGameState() is IMatchGameState matchGameState) {
      var time = TimeSpan.FromMilliseconds(matchGameState.GameTimeMSec);
      TimePlayedLabel.Text = FormatTimeDynamic(time.TotalSeconds);
    }
  }

  public static string FormatTimeDynamic(double totalSeconds) {
    var time = TimeSpan.FromSeconds(totalSeconds);

    if (time.TotalHours >= 1) {
      return $"{(int)time.TotalHours}h {time.Minutes}m {time.Seconds}s";
    }
    else if (time.TotalMinutes >= 1) {
      return $"{time.Minutes}m {time.Seconds}s";
    }
    else {
      return $"{time.Seconds}s";
    }
  }
}
