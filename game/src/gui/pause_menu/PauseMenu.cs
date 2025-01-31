namespace HOB;

using Godot;
using System;

public partial class PauseMenu : CanvasLayer, IPauseMenu {
  [Export] private SettingsMenu SettingsMenu { get; set; }
  public event Action ResumeEvent;
  public event Action MainMenuEvent;
  public event Action QuitEvent;

  public override void _Ready() {
    base._Ready();

    SettingsMenu.Closed += () => SettingsMenu.Visible = false;
  }

  private void OnResumePressed() {
    ResumeEvent?.Invoke();
  }

  private void OnSettingsPressed() {
    SettingsMenu.Visible = true;
  }

  private void OnMainMenuPressed() {
    MainMenuEvent?.Invoke();
  }

  private void OnQuitPressed() {
    QuitEvent?.Invoke();
  }

}
