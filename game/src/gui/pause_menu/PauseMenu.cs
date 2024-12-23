namespace HOB;

using Godot;
using System;

public partial class PauseMenu : Control {
  [Signal] public delegate void ResumeEventHandler();
  [Signal] public delegate void SettingsEventHandler();
  [Signal] public delegate void MainMenuEventHandler();
  [Signal] public delegate void QuitEventHandler();

  private void OnResumePressed() {
    EmitSignal(SignalName.Resume);
  }

  private void OnSettingsPressed() {
    EmitSignal(SignalName.Settings);
  }

  private void OnMainMenuPressed() {
    EmitSignal(SignalName.MainMenu);
  }

  private void OnQuitPressed() {
    EmitSignal(SignalName.Quit);
  }
}
