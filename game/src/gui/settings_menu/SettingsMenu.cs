namespace HOB;

using Godot;
using System;

public partial class SettingsMenu : Control {
  [Signal] public delegate void ClosedEventHandler();
  [Export] private OptionButton _resolutionOptionButton;
  [Export] private OptionButton _screenModeOptionButton;
  [Export] private CheckBox _borderlessCheckbox;
  [Export] private CheckBox _vsyncCheckbox;
  [Export] private Slider _fpsLimitSlider;
  [Export] private Label _fpsLimitLabel;

  public string[] Resolutions { get; private set; } = [
          "1152x648",
          "1280x1024",
          "1360x768",
          "1366x768",
          "1440x900",
          "1600x900",
          "1680x1050",
          "1920x1080"
  ];
  public string[] ScreenModes { get; private set; } = [
    "Windowed",
          "Fullscreen"
  ];

  private SettingsManager SettingsManager;

  public override void _Ready() {
    SettingsManager = GetNode<SettingsManager>("/root/SettingsManager");

    InitializeScreenModeOptions();
    InitializeResolutionOptions();
    InitializeBorderlessCheckbox();
    InitializeVsyncCheckbox();
    InitializeFpsLimit();
  }

  public override void _Process(double delta) {
    if (Input.IsActionJustPressed(BuiltinInputActions.UICancel)) {
      OnClosePressed();
    }
  }

  private void InitializeScreenModeOptions() {
    _screenModeOptionButton.Clear();

    foreach (var screenMode in ScreenModes) {
      _screenModeOptionButton.AddItem(screenMode);
    }

    var currentMode = GetWindow().Mode == Window.ModeEnum.Fullscreen ? "Fullscreen" : "Windowed";
    var index = Array.IndexOf(ScreenModes, currentMode);
    if (index >= 0) {
      _screenModeOptionButton.Selected = index;
    }
  }

  private void InitializeResolutionOptions() {
    _resolutionOptionButton.Clear();

    if (GetWindow().Mode == Window.ModeEnum.Windowed) {
      foreach (var resolution in Resolutions) {
        _resolutionOptionButton.AddItem(resolution);
      }

      var windowSize = GetWindow().Size;
      var currentResolution = $"{windowSize.X}x{windowSize.Y}";
      var index = Array.IndexOf(Resolutions, currentResolution);
      if (index >= 0) {
        _resolutionOptionButton.Selected = index;
      }

      _resolutionOptionButton.Disabled = false;
    }
    else {
      _resolutionOptionButton.Disabled = true;
    }
  }

  private void InitializeBorderlessCheckbox() => _borderlessCheckbox.ButtonPressed = GetWindow().Borderless;

  private void InitializeVsyncCheckbox() => _vsyncCheckbox.ButtonPressed = DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Enabled;

  private void InitializeFpsLimit() {
    var maxFps = Engine.MaxFps;
    _fpsLimitSlider.Value = maxFps == 0 ? 250 : maxFps;
    _fpsLimitLabel.Text = $"FPS Limit : {(maxFps is 0 or 250? "Unlimited" : maxFps)}";
  }


  public void OnClosePressed() => EmitSignal(SignalName.Closed);

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

      _resolutionOptionButton.Disabled = true;
    }
    else {
      _resolutionOptionButton.Disabled = false;
      if (_resolutionOptionButton.GetItemCount() > 0) {
        var selectedIndex = _resolutionOptionButton.Selected;
        OnResolutionOptionButtonPressed(selectedIndex);
      }
    }

    // Zapisz ustawienia
    SettingsManager.SaveSettings();

    InitializeResolutionOptions();
  }

  private void OnBorderlessCheckboxToggled(bool buttonPressed) {
    GetWindow().Borderless = buttonPressed;
    SettingsManager.SaveSettings();
  }

  private void OnVsyncCheckboxToggled(bool buttonPressed) {

    DisplayServer.WindowSetVsyncMode(buttonPressed ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
    SettingsManager.SaveSettings();
  }

  private void OnFpsLimiterSliderValueChanged(float value) {
    Engine.MaxFps = (int)value == 250 ? 0 : (int)value;
    _fpsLimitLabel.Text = $"FPS Limit : {(value is 0 or 250? "Unlimited" : value)}";
    SettingsManager.SaveSettings();
  }
}
