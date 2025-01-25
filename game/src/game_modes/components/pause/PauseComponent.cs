namespace HOB;

using System;
using GameplayFramework;
using Godot;

[GlobalClass]
public partial class PauseComponent : GameModeComponent {
  [Export] public PackedScene PauseMenuScene { get; private set; }

  private PauseMenu _pauseMenu;

  private bool _pauseMenuShown;

  public override void _Ready() {
    base._Ready();

    _pauseMenu = PauseMenuScene.Instantiate<PauseMenu>();
    _pauseMenu.Visible = false;

    AddChild(_pauseMenu);
  }

  public void Resume() {
    _pauseMenu.Visible = false;
    if (GetGameState().PauseGame) {
      Game.SetPause(false);
    }

    _pauseMenuShown = false;
  }
  public void Pause() {
    if (_pauseMenuShown) {
      return;
    }
    _pauseMenu.Visible = true;
    _pauseMenuShown = true;

    if (GetGameState().PauseGame) {
      Game.SetPause(true);
    }
  }

  public override IPauseGameState GetGameState() => base.GetGameState() as IPauseGameState;
  public IPauseMenu GetPauseMenu() => _pauseMenu;
}
