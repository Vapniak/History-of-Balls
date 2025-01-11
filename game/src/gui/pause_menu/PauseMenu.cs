namespace HOB;

using Godot;
using System;

public partial class PauseMenu : CanvasLayer, IPauseMenu {
  // BUG: when panku console is opened when game is paused it unpauses it
  [Export] private SettingsMenu SettingsMenu { get; set; }
  public Action Resume { get; set; }

  public Action MainMenu { get; set; }

  public Action Quit { get; set; }

  public override void _Ready() {
    base._Ready();

    SettingsMenu.Closed += () => SettingsMenu.Visible = false;
  }

  private void OnResumePressed() {
    Resume?.Invoke();
  }

  private void OnSettingsPressed() {
    SettingsMenu.Visible = true;
  }

  private void OnMainMenuPressed() {
    MainMenu?.Invoke();
  }

  private void OnQuitPressed() {
    Quit?.Invoke();
  }

}
