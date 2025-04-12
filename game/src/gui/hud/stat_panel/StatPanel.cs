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

  public void UpdateEntry(string name, string value, Texture2D? icon, Color? color) {
    StatPanelEntry entry;
    if (Entries.TryGetValue(name, out entry!)) {

    }
    else {
      entry = EntryScene!.InstantiateOrNull<StatPanelEntry>();
      entry.SetEntryName(name + ": ");
    }

    entry.SetEntryValue(value);
    entry.SetIcon(icon, color);



    if (Entries.TryAdd(name, entry)) {
      EntriesList!.AddChild(entry);
    }

    // FIXME: the panel doesnt resize to fit labels
  }
}
