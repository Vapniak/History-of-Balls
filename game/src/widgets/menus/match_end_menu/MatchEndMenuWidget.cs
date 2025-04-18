namespace HOB;

using GameplayFramework;
using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class MatchEndMenuWidget : Widget, IWidgetFactory<MatchEndMenuWidget> {
  [Export] private Label WinnerTextLabel { get; set; }
  [Export] private Label TimePlayedLabel { get; set; }

  static MatchEndMenuWidget IWidgetFactory<MatchEndMenuWidget>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://cnww5rxeofin3").Instantiate<MatchEndMenuWidget>();
  }

  public void OnGameEnd(IMatchController controller) {
    WinnerTextLabel.Text = "WINNER: " + controller.GetPlayerState().PlayerName;
    var time = TimeSpan.FromMilliseconds(controller.GetGameState().GameTimeMSec);
    TimePlayedLabel.Text = $"TIME PLAYED: {time.Minutes:00}:{time.Seconds:00}";
  }

  private void GoToMainMenu() {
    _ = GameInstance.GetWorld().OpenLevel("main_menu");
  }
}
