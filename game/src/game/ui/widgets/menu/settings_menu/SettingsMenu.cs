namespace HOB;

using Godot;
using WidgetSystem;

public partial class SettingsMenu : HOBWidget, IWidgetFactory<SettingsMenu> {
  static SettingsMenu IWidgetFactory<SettingsMenu>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bxauf3xyrcdj").Instantiate<SettingsMenu>();
  }
}
