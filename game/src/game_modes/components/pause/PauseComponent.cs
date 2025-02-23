namespace HOB;

using System;
using GameplayFramework;
using Godot;

[GlobalClass]
public partial class PauseComponent : GameModeComponent {
  [Export] public PauseMenu PauseMenu { get; private set; }

  public override void _Ready() {
    base._Ready();

    PauseMenu.Visible = false;
  }

  public void HidePauseMenu() {
    PauseMenu.Visible = false;
  }
  public void ShowPauseMenu() {
    PauseMenu.Visible = true;
  }

  public override IPauseGameState GetGameState() => base.GetGameState() as IPauseGameState;
  public IPauseMenu GetPauseMenu() => PauseMenu;
}
