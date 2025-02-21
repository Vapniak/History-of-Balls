namespace HOB;

using Godot;
using System;

public partial class StatPanel : Control {
  [Export] private Label NameLabel { get; set; }
  [Export] private PackedScene EntryScene { get; set; }
  [Export] private Control Entries { get; set; }

  public void SetNameLabel(string name) {
    NameLabel.Text = name;
  }
  public void ClearEntries() {
    foreach (var child in Entries.GetChildren()) {
      child.QueueFree();
    }
  }

  public void AddEntry(string name, string value) {
    var entry = EntryScene.Instantiate<StatPanelEntry>();
    entry.SetEntryName(name);
    entry.SetEntryValue(value);

    Entries.AddChild(entry);
  }
}
