namespace HOB;

using Godot;
using System;

public partial class SettingsMenu : Control {
  [Signal] public delegate void ClosedEventHandler();

  public string[] Resolutions = new string[] {
          "1152x648",
          "1280x1024",
          "1360x768",
          "1366x768",
          "1440x900",
          "1600x900",
          "1680x1050",
          "1920x1080",
      };
  public string[] ScreenModes = new string[] {
          "Windowed",
          "Fullscreen",
      };

  private OptionButton ResolutionOptionButton;
  private OptionButton ScreenModeOptionButton;
  private SettingsManager SettingsManager => GetNode<SettingsManager>("/root/SettingsManager");

  public override void _Ready() {
    ResolutionOptionButton = GetNode<OptionButton>("MarginContainer/TabContainer/Video/VBoxContainer/ResolutionOption");
    ScreenModeOptionButton = GetNode<OptionButton>("MarginContainer/TabContainer/Video/VBoxContainer/ScreenModeOption");

    InitializeScreenModeOptions();
    InitializeResolutionOptions();
  }

  public override void _Process(double delta) {
    if (Input.IsActionJustPressed(BuiltinInputActions.UICancel)) {
      OnClosePressed();
    }
  }

  private void InitializeScreenModeOptions() {
    ScreenModeOptionButton.Clear();

    foreach (var screenMode in ScreenModes) {
      ScreenModeOptionButton.AddItem(screenMode);
    }

    var currentMode = GetWindow().Mode == Window.ModeEnum.Fullscreen ? "Fullscreen" : "Windowed";
    var index = Array.IndexOf(ScreenModes, currentMode);
    if (index >= 0) {
      ScreenModeOptionButton.Selected = index;
    }
  }

  private void InitializeResolutionOptions() {
    ResolutionOptionButton.Clear();

    if (GetWindow().Mode == Window.ModeEnum.Windowed) {
      foreach (var resolution in Resolutions) {
        ResolutionOptionButton.AddItem(resolution);
      }

      var windowSize = GetWindow().Size;
      var currentResolution = $"{windowSize.X}x{windowSize.Y}";
      var index = Array.IndexOf(Resolutions, currentResolution);
      if (index >= 0) {
        ResolutionOptionButton.Selected = index;
      }

      ResolutionOptionButton.Disabled = false;
    }
    else {
      ResolutionOptionButton.Disabled = true;
    }
  }

  public void OnClosePressed() {
    EmitSignal(SignalName.Closed);
  }

  private void OnResolutionOptionButtonPressed(long index) {
    if (GetWindow().Mode != Window.ModeEnum.Windowed) {
      return;
    }

    var selectedResolution = Resolutions[index];
    var resolution = selectedResolution.Split('x');
    var width = int.Parse(resolution[0]);
    var height = int.Parse(resolution[1]);

    GetWindow().Size = new Vector2I(width, height);

    SettingsManager.CenterWindowOnCurrentMonitor();
    SettingsManager.SaveSettings();
  }

  private void OnScreenModeOptionButtonPressed(long index) {
    var selectedMode = ScreenModes[index];
    var mode = selectedMode == "Fullscreen" ? Window.ModeEnum.Fullscreen : Window.ModeEnum.Windowed;
    GetWindow().Mode = mode;

    if (mode == Window.ModeEnum.Fullscreen) {
      var monitorIndex = GetWindow().CurrentScreen;
      var screenSize = DisplayServer.ScreenGetSize(monitorIndex);
      GetWindow().Size = screenSize;
      GetWindow().Position = Vector2I.Zero;

      ResolutionOptionButton.Disabled = true;
    }
    else {
      ResolutionOptionButton.Disabled = false;

      if (ResolutionOptionButton.GetItemCount() > 0) {
        var selectedIndex = ResolutionOptionButton.Selected;
        OnResolutionOptionButtonPressed(selectedIndex);
      }
    }

    // Zapisz ustawienia
    SettingsManager.SaveSettings();

    InitializeResolutionOptions();
  }
}
