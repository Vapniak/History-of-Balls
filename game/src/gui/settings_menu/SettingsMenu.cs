namespace HOB;

using Godot;
using System;

public partial class SettingsMenu : Control {
  [Signal] public delegate void ClosedEventHandler();

  // TODO: settings menu
  public override void _Process(double delta) {
    if (Input.IsActionJustPressed(BuiltinInputActions.UICancel)) {
      OnClosePressed();
    }
  }
  public void OnClosePressed() {
    EmitSignal(SignalName.Closed);
  }
}
