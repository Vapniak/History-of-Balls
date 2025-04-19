namespace HOB;

using GameplayFramework;
using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class TimePlayedWidget : Widget {
  [Export] private Label TimePlayedLabel { get; set; } = default!;

  public override void _Ready() {
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
      TimePlayedLabel.Text = $"{time.Minutes:00}:{time.Seconds:00}";
    }
  }
}
