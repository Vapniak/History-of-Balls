namespace HOB;

using Godot;
using System;
using WidgetSystem;

public partial class MatchEndMenu : Widget, IWidget<MatchEndMenu> {
  [Export] private Label WinnerTextLabel { get; set; }
  [Export] private Label TimePlayedLabel { get; set; }

  static MatchEndMenu IWidget<MatchEndMenu>.Create() {
    return ResourceLoader.Load<PackedScene>("uid://cnww5rxeofin3").Instantiate<MatchEndMenu>();
  }

  public void OnGameEnd(IMatchController controller) {
    WinnerTextLabel.Text = "WINNER: " + controller.GetPlayerState().PlayerName;
    var time = TimeSpan.FromMilliseconds(controller.GetGameState().GameTimeMSec);
    TimePlayedLabel.Text = $"TIME PLAYED: {time.Minutes:00}:{time.Seconds:00}";
  }
}
