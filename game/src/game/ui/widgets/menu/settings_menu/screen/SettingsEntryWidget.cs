namespace HOB;

using Godot;
using HOB;
using System;
using WidgetSystem;

public partial class SettingsEntryWidget : HOBWidget, IWidgetFactory<SettingsEntryWidget> {
  [Export] public Label SettingNameLabel { get; private set; } = default!;
  public static SettingsEntryWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://b7vf6elxpqnns").Instantiate<SettingsEntryWidget>();
  }
}
