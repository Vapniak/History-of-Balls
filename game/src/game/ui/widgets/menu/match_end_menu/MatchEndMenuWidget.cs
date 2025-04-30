namespace HOB;

using GameplayFramework;
using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class MatchEndMenuWidget : HOBWidget, IWidgetFactory<MatchEndMenuWidget> {
  [Export] private Label WinnerTextLabel { get; set; } = default!;
  [Export] private Label TimePlayedLabel { get; set; } = default!;

  static MatchEndMenuWidget IWidgetFactory<MatchEndMenuWidget>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://cnww5rxeofin3").Instantiate<MatchEndMenuWidget>();
  }

  public void OnGameEnd(IMatchController controller) {
    WinnerTextLabel.Text = "WINNER: " + controller.GetPlayerState().PlayerName;
    var time = TimeSpan.FromMilliseconds(controller.GetGameState().GameTimeMSec);
    TimePlayedLabel.Text = $"TIME PLAYED: {TimePlayedWidget.FormatTimeDynamic(time.TotalSeconds)}";
  }

  private void GoToMainMenu() {
    _ = GameInstance.GetWorld().OpenLevel("main_menu_level");
  }
}
