namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;

public partial class Splash : Control {

  [Export] private VideoStreamPlayer _videoStreamPlayer;

  public override void _Ready() {
    _videoStreamPlayer.Finished += GoToMainMenu;

    PlayIntro();
  }
  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed) {
      GoToMainMenu();
    }
  }
  public void PlayIntro() {
    _videoStreamPlayer.Play();
  }
  private static void GoToMainMenu() => Game.CreateWorld("main_menu_level");
}
