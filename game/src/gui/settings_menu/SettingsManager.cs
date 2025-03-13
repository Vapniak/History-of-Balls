namespace HOB;

using Godot;
using System;

public partial class SettingsManager : Node {
  private readonly string _configFilePath = "user://settings.cfg";
  private ConfigFile _configFile;

  public override void _Ready() {
    _configFile = new ConfigFile();
    var error = _configFile.Load(_configFilePath);

    if (error != Error.Ok) {
      GD.Print("Settings file doesn't exist. Creating with defaults...");
      SetDefaultSettings();
    } else {
      LoadSettings();
    }
  }

  private void SetDefaultSettings() {
    GetWindow().Mode = Window.ModeEnum.Windowed;
    GetWindow().Size = new Vector2I(1280, 720);
    GetWindow().Borderless = false;
    DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);
    Engine.MaxFps = 60;

    AudioServer.SetBusVolumeDb(0, 0);

    CenterWindowOnCurrentMonitor();
    SaveSettings();
  }

  public void SaveSettings() {
    var mode = GetWindow().Mode == Window.ModeEnum.Fullscreen ? "Fullscreen" : "Windowed";
    var width = GetWindow().Size.X;
    var height = GetWindow().Size.Y;

    _configFile.SetValue("display", "screen_mode", mode);
    _configFile.SetValue("display", "resolution", $"{width}x{height}");
    _configFile.SetValue("display", "vsync", DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Enabled ? "Enabled" : "Disabled");
    _configFile.SetValue("display", "borderless", GetWindow().Borderless.ToString().ToLower());
    _configFile.SetValue("display", "fps_limit", Engine.MaxFps);

    _configFile.SetValue("audio", "master_volume_db", AudioServer.GetBusVolumeDb(0));

    var error = _configFile.Save(_configFilePath);
    if (error != Error.Ok) {
      GD.PrintErr($"Problem saving settings: {error}");
    }
  }

  public void LoadSettings() {
    try {
      var modeValue = (string)_configFile.GetValue("display", "screen_mode", "Windowed");
      var mode = modeValue == "Fullscreen" ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;

      var resolutionValue = (string)_configFile.GetValue("display", "resolution", "1280x720");
      var resolution = resolutionValue.Split('x');
      var width = int.Parse(resolution[0]);
      var height = int.Parse(resolution[1]);

      var vsyncValue = (string)_configFile.GetValue("display", "vsync", "Disabled");
      var vsync = vsyncValue == "Enabled" ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled;
      DisplayServer.WindowSetVsyncMode(vsync);

      var borderlessValue = (string)_configFile.GetValue("display", "borderless", "false");
      var borderless = bool.Parse(borderlessValue);
      var fpsLimit = (int)_configFile.GetValue("display", "fps_limit", 60);
      Engine.MaxFps = fpsLimit;

      float volumeDb = (float)_configFile.GetValue("audio", "master_volume_db", 0.0f);
      AudioServer.SetBusVolumeDb(0, volumeDb);

      GetWindow().Size = new Vector2I(width, height);

      if (mode == Window.ModeEnum.Windowed) {
        GetWindow().Borderless = borderless;
        CenterWindowOnCurrentMonitor();
      }


      GetWindow().Mode = mode;

      if (mode == Window.ModeEnum.Fullscreen) {

        GetWindow().Position = Vector2I.Zero;
      }
    }
    catch (Exception e) {
      GD.PrintErr($"Error loading settings: {e.Message}");
      SetDefaultSettings();
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
