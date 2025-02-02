namespace HOB;

using Godot;
using System;

public partial class StatPanelEntry : Control {
  [Export] private Label EntryNameLabel { get; set; }
  [Export] private Label EntryValueLabel { get; set; }

  public void SetEntryName(string name) {
    EntryNameLabel.Text = name;
  }

  public void SetEntryValue(string value) {
    EntryValueLabel.Text = value;
  }
}
