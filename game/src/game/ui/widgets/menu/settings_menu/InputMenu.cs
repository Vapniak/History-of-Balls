namespace HOB;

using Godot;
using System;


public partial class InputMenu : Node {
  [Export] private Control ControlsEntries { get; set; } = default!;

  public override void _Ready() {
    foreach (var action in InputMap.GetActions()) {
      var widget = SettingsEntryWidget.CreateWidget();
      widget.SettingNameLabel.Text = action;

      var events = new HBoxContainer();
      foreach (var inputEvent in InputMap.ActionGetEvents(action)) {
        var label = new Label();
        label.Text = inputEvent.AsText();
        events.AddChild(label);
      }

      widget.AddChild(events);
      ControlsEntries.AddChild(widget);
    }
  }
}
