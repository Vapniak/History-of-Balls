namespace HOB;

using GameplayFramework;
using Godot;
using System;

public partial class MainMenu : Control {
  [Export] private ParallaxBackground ParallaxBackground { get; set; }

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

    ParallaxBackground.ScrollOffset = GetViewport().GetMousePosition();
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseMotion motion) {
      ParallaxBackground.ScrollOffset -= motion.Relative;
    }
  }


  private void OnStartButtonPressed() {
    GameInstance.GetGameMode<MainMenuGameMode>().StartGame();
  }

  private void OnSettingsButtonPressed() {
    GameInstance.GetGameMode<MainMenuGameMode>().OpenSettings();
  }

  private void OnTutorialButtonPressed() {
    GameInstance.GetGameMode<MainMenuGameMode>().OpenTutorial();
  }

  private void OnQuitButtonPressed() {
    GameInstance.GetGameMode<MainMenuGameMode>().Quit();
  }
}
