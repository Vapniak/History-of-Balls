namespace HOB;

using Godot;
using System;

public partial class PauseMenu : CanvasLayer {
  [Export] private SettingsMenu SettingsMenu { get; set; }
  public event Action ResumeEvent;
  public event Action MainMenuEvent;
  public event Action QuitEvent;

  public override void _Ready() {
    base._Ready();

    SettingsMenu.Closed += () => SettingsMenu.Visible = false;
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);

    if (Visible && @event.IsActionPressed(BuiltinInputActions.UICancel)) {
      GetViewport().SetInputAsHandled();
      OnResumePressed();
    }
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
