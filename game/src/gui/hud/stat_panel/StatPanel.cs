namespace HOB;

using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class StatPanel : Control {
  [Export] private Label? NameLabel { get; set; }
  [Export] private PackedScene? EntryScene { get; set; }
  [Export] private Control? EntriesList { get; set; }
  [Export] private TextureRect? IconTextureRect { get; set; }

  private Godot.Collections.Dictionary<string, StatPanelEntry> Entries { get; set; } = new();

  public void SetNameLabel(string name) {
    NameLabel.Text = name;
  }

  public void SetIcon(Texture2D icon) {
    IconTextureRect.Texture = icon;
  }
  public void ClearEntries() {
    Entries.Clear();
    foreach (var child in EntriesList.GetChildren()) {
      child.Free();
    }
  }

  public void AddEntry(string name, string value) {
    var entry = EntryScene.Instantiate<StatPanelEntry>();
    entry.SetEntryName(name);
    entry.SetEntryValue(value);

    if (Entries.TryAdd(name, entry)) {
      EntriesList.AddChild(entry);
    }
  }

  public void UpdateEntry(string name, string value) {
    if (Entries.TryGetValue(name, out var entry)) {
      entry.SetEntryValue(value);
    }
  }
}
