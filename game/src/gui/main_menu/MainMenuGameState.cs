namespace HOB;

using Godot;
using HOB.Core;

[GlobalClass]
public partial class MainMenuGameState : GameState {
  [Signal] public delegate void SplashScreenSkippedEventHandler();

  public void SkipSplashScreen() => EmitSignal(SignalName.SplashScreenSkipped);
}
