namespace HOB;

using GameplayFramework;
using Godot;
using System;

public partial class MainMenu : Control {
  [Export] private VideoStreamPlayer VideoStreamPlayer { get; set; }
  [Export] private ParallaxBackground ParallaxBackground { get; set; }

  public static bool FirstTimeOpening { get; private set; } = true;

  public override void _Ready() {
    var value = 0.00f;
    var count = 0;
    foreach (var child in ParallaxBackground.GetChildren()) {
      if (child is ParallaxLayer layer) {

        if (count == 0) {
          value = 2f;
        }
        else if (count == 8) {
          value = 13f;
        }

        layer.MotionScale = new Vector2(0.001f, 0.0002f) * value;

        if (count is >= 0 and <= 7) {
          value += 2f;
        }
        else {
          value += 4f;
        }
        count++;
      }
    }

    if (FirstTimeOpening) {
      VideoStreamPlayer.Finished += FadeInMenu;

      VideoStreamPlayer.Play();
    }
    else {
      FadeInMenu();
    }

    FirstTimeOpening = false;
  }

  public override void _Process(double delta) {
    ParallaxBackground.ScrollOffset = GetViewport().GetMousePosition();
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && VideoStreamPlayer.IsPlaying()) {
      FadeInMenu();
      GetViewport().SetInputAsHandled();
    }
  }

  private void FadeInMenu() {
    VideoStreamPlayer.Stop();
    VideoStreamPlayer.Hide();
  }
  private void OnStartButtonPressed() {
    Game.GetGameMode<MainMenuGameMode>().StartGame();
  }

  private void OnSettingsButtonPressed() {
    Game.GetGameMode<MainMenuGameMode>().OpenSettings();
  }

  private void OnQuitButtonPressed() {
    Game.GetGameMode<MainMenuGameMode>().Quit();
  }
}
