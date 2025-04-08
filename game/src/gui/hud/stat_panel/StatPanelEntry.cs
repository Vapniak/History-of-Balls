namespace HOB;

using Godot;
using System;

public partial class StatPanelEntry : Control {
  [Export] private Label EntryNameLabel { get; set; }
  [Export] private Label EntryValueLabel { get; set; }
  [Export] private TextureRect IconTexture { get; set; }

  public void SetEntryName(string name) {
    EntryNameLabel.Text = name;
    EntryNameLabel.ResetSize();
    ResetSize();
  }

  public void SetIcon(Texture2D? icon) {
    IconTexture.Texture = icon;
  }

  public void SetEntryValue(string value) {
    EntryValueLabel.Text = value;
    EntryValueLabel.ResetSize();
    ResetSize();
  }
}
