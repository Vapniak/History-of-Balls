namespace HOB;

using Godot;
using System;

public partial class SettingsManager : Node {
  private readonly string _configFilePath = "user://settings.cfg";
  private ConfigFile _configFile;

  public override void _Ready() {
    _configFile = new ConfigFile();
    LoadSettings();
  }

  public void SaveSettings() {
    var mode = GetWindow().Mode == Window.ModeEnum.Fullscreen ? "Fullscreen" : "Windowed";
    var width = GetWindow().Size.X;
    var height = GetWindow().Size.Y;

    _configFile.SetValue("display", "screen_mode", mode);
    _configFile.SetValue("display", "resolution", $"{width}x{height}");

    var error = _configFile.Save(_configFilePath);
    if (error != Error.Ok) {
      GD.PrintErr("File saving problem :" + error);
    }
  }

  public void LoadSettings() {
    var error = _configFile.Load(_configFilePath);
    if (error != Error.Ok) {
      GD.Print("Settings file is not exist!");
      return;
    }

    var modeValue = (string)_configFile.GetValue("display", "screen_mode", "Fullscreen");
    var mode = modeValue == "Fullscreen" ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
    GetWindow().Mode = mode;

    var resolutionValue = (string)_configFile.GetValue("display", "resolution", "1920x1080");
    var resolution = resolutionValue.Split('x');
    var width = int.Parse(resolution[0]);
    var height = int.Parse(resolution[1]);

    if (mode == Window.ModeEnum.Windowed) {
      GetWindow().Size = new Vector2I(width, height);
      CenterWindowOnCurrentMonitor();
    }
    else {
      var monitorIndex = GetWindow().CurrentScreen;
      var screenSize = DisplayServer.ScreenGetSize(monitorIndex);
      GetWindow().Size = screenSize;
      GetWindow().Position = Vector2I.Zero;
    }
  }

  public void CenterWindowOnCurrentMonitor() {
    var windowSize = GetWindow().Size;
    var monitorIndex = GetWindow().CurrentScreen;
    var monitorPosition = DisplayServer.ScreenGetPosition(monitorIndex);
    var monitorSize = DisplayServer.ScreenGetSize(monitorIndex);
    var position = monitorPosition + ((monitorSize - windowSize) / 2);
    GetWindow().Position = position;
  }
}
