namespace HOB;

using Godot;
using Godot.Collections;
using System;

public partial class StatPanel : Control {
  [Export] private Label NameLabel { get; set; }
  [Export] private PackedScene EntryScene { get; set; }
  [Export] private Control EntriesList { get; set; }

  private Dictionary<string, StatPanelEntry> Entries { get; set; } = new();

  public void SetNameLabel(string name) {
    NameLabel.Text = name;
  }
  public void ClearEntries() {
    Entries.Clear();
    foreach (var child in EntriesList.GetChildren()) {
      child.QueueFree();
    }
  }

  public void AddEntry(string name, string value) {
    var entry = EntryScene.Instantiate<StatPanelEntry>();
    entry.SetEntryName(name);
    entry.SetEntryValue(value);

    Entries.Add(name, entry);

    EntriesList.AddChild(entry);
  }

  public void UpdateEntry(string name, string value) {
    if (Entries.TryGetValue(name, out var entry)) {
      entry.SetEntryValue(value);
    }
  }
}
