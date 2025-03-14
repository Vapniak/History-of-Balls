namespace HOB;

using Godot;
using System;

public partial class SettingsMenu : Control {
  [Signal] public delegate void ClosedEventHandler();

  public override void _Process(double delta) {
    if (Input.IsActionJustPressed(BuiltinInputActions.UICancel)) {
      OnClosePressed();
    }
  }
  public void OnClosePressed() => EmitSignal(SignalName.Closed);
}
