namespace HOB;

using Godot;
using System;
using System.Linq;

public partial class InputMenu : Node {
  [Export] private Control ControlsEntries { get; set; } = default!;

  public override void _Ready() {
    foreach (var action in InputMap.GetActions()) {
      if (action.ToString().StartsWith("ui_")) {
        continue;
      }

      var widget = SettingsEntryWidget.CreateWidget();
      widget.SettingNameLabel.Text = action.ToString();

      var events = new Label();

      var text = "";
      var actions = InputMap.ActionGetEvents(action);
      foreach (var inputEvent in actions) {
        text += GetInputEventText(inputEvent);

        if (actions.Last() != inputEvent) {
          text += " | ";
        }
      }
      events.Text = text;

      widget.AddChild(events);
      ControlsEntries.AddChild(widget);
    }
  }

  private string GetInputEventText(InputEvent inputEvent) {
    return inputEvent switch {
      InputEventKey keyEvent => OS.GetKeycodeString(keyEvent.Keycode),
      InputEventMouseButton mouseEvent => $"Mouse {mouseEvent.ButtonIndex}",
      InputEventJoypadButton joypadEvent => $"Joypad {joypadEvent.ButtonIndex}",
      InputEventJoypadMotion motionEvent => $"Axis {motionEvent.Axis}",
      _ => inputEvent.AsText()
    };
  }
}
