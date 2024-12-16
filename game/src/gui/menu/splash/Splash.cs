namespace HOB;

using Godot;
using HOB.Core;
using System;

public partial class Splash : Control {
  [Export] private AnimationPlayer _animationPlayer;

  public override void _Ready() {
    _animationPlayer.AnimationFinished += OnAnimationFinished;
  }

  public override void _ExitTree() {
    _animationPlayer.AnimationFinished -= OnAnimationFinished;
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed) {
      Game.GetGameState<MainMenuGameState>().SkipSplashScreen();
    }
  }

  private void OnAnimationFinished(StringName name) {
    Game.GetGameState<MainMenuGameState>().SkipSplashScreen();
  }
}
