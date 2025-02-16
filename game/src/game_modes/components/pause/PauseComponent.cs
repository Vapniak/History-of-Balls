namespace HOB;

using System;
using GameplayFramework;
using Godot;

[GlobalClass]
public partial class PauseComponent : GameModeComponent {
  [Export] public PauseMenu PauseMenu { get; private set; }

  private bool _pauseMenuShown;

  public override void _Ready() {
    base._Ready();

    PauseMenu.Visible = false;
  }

  public void Resume() {
    PauseMenu.Visible = false;
    if (GetGameState().PauseGame) {
      GameInstance.SetPause(false);
    }

    _pauseMenuShown = false;
  }
  public void Pause() {
    if (_pauseMenuShown) {
      return;
    }
    PauseMenu.Visible = true;
    _pauseMenuShown = true;

    if (GetGameState().PauseGame) {
      GameInstance.SetPause(true);
    }
  }

  public override IPauseGameState GetGameState() => base.GetGameState() as IPauseGameState;
  public IPauseMenu GetPauseMenu() => PauseMenu;
}
