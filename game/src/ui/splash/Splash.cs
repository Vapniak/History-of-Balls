namespace HOB;

using Godot;

public partial class Splash : Control {
  [Export] private PackedScene _mainMenuScene;
  [Export] private AnimationPlayer _animationPlayer;

  public override void _EnterTree() {
    _animationPlayer.AnimationFinished += OnAnimationFinished;
  }


  public override void _Ready() {
  }

  public override void _ExitTree() {
    _animationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed) {
      GoToMainMenu();
    }
  }

  public void PlayIntro() => _animationPlayer.Play("intro");

  private void OnAnimationFinished(StringName name) {
    GoToMainMenu();
  }

  private void GoToMainMenu() {
    GetTree().ChangeSceneToPacked(_mainMenuScene);
  }
}
