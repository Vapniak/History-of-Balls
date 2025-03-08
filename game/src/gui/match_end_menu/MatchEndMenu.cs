namespace HOB;

using Godot;
using System;

public partial class MatchEndMenu : CanvasLayer {
  [Export] private Label WinnerTextLabel { get; set; }
  [Export] private Label TimePlayedLabel { get; set; }

  public void OnGameEnd(IMatchController controller) {
    Show();
    WinnerTextLabel.Text = "WINNER: " + controller.GetPlayerState().PlayerName;
    var time = TimeSpan.FromMilliseconds(Time.GetTicksMsec() - controller.GetGameState().GameStartTicksMSec);
    TimePlayedLabel.Text = "TIME PLAYED: " + time.Minutes.ToString() + " MIN";
  }
}
