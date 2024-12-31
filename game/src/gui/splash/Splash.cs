namespace HOB;

using GameplayFramework;
using Godot;

public partial class Splash : Control {
  [Export] private AnimationPlayer _animationPlayer;

  public override void _Ready() => PlayIntro();

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed) {
      GoToMainMenu();
    }
  }
  public void PlayIntro() => _animationPlayer.Play("intro");
  private static void GoToMainMenu() => Game.CreateWorld("main_menu_level");
}
