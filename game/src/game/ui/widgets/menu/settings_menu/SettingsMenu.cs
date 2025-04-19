namespace HOB;

using Godot;
using System;
using HOB;
using WidgetSystem;

public partial class SettingsMenu : Widget, IWidgetFactory<SettingsMenu> {
  [Export] private OptionButton _resolutionOptionButton;
  [Export] private OptionButton _screenModeOptionButton;
  [Export] private CheckBox _borderlessCheckbox;
  [Export] private CheckBox _vsyncCheckbox;
  [Export] private Slider _fpsLimitSlider;
  [Export] private Label _fpsLimitLabel;
  [Export] private Slider _VolumeLimitSlider;
  [Export] private Label _VolumeLimitLabel;

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

  private SettingsManager SettingsManager { get; set; }

  public override void _Ready() {
    SettingsManager = GetNode<SettingsManager>("/root/SettingsManager");

    InitializeScreenModeOptions();
    InitializeResolutionOptions();
    InitializeBorderlessCheckbox();
    InitializeVsyncCheckbox();
    InitializeFpsLimit();
    InitializeVolumeControl();

    UpdateFpsLimitAvailability(DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Enabled);
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

    UpdateScreenModeDependentControls(GetWindow().Mode == Window.ModeEnum.Fullscreen);
  }

  private void InitializeResolutionOptions() {
    _resolutionOptionButton.Clear();

    foreach (var resolution in Resolutions) {
      _resolutionOptionButton.AddItem(resolution);
    }

    var windowSize = GetWindow().Size;
    var currentResolution = $"{windowSize.X}x{windowSize.Y}";

    var index = Array.IndexOf(Resolutions, currentResolution);
    if (index >= 0) {
      _resolutionOptionButton.Selected = index;
    }
    else {
      _resolutionOptionButton.AddItem(currentResolution);
      _resolutionOptionButton.Selected = _resolutionOptionButton.ItemCount - 1;
    }
  }

  private void InitializeBorderlessCheckbox() {
    _borderlessCheckbox.ButtonPressed = GetWindow().Borderless;
    UpdateBorderlessAvailability(GetWindow().Mode == Window.ModeEnum.Fullscreen);
  }

  private void InitializeVsyncCheckbox() {
    _vsyncCheckbox.ButtonPressed = DisplayServer.WindowGetVsyncMode() == DisplayServer.VSyncMode.Enabled;
  }

  private void InitializeFpsLimit() {
    var maxFps = Engine.MaxFps;
    _fpsLimitSlider.Value = maxFps == 0 ? 250 : maxFps;
    _fpsLimitLabel.Text = $"FPS Limit: {(maxFps is 0 or 250 ? "Unlimited" : maxFps.ToString())}";
  }

  private void InitializeVolumeControl() {
    float volumeDb = AudioServer.GetBusVolumeDb(0);
    float volumePercent = Mathf.Pow(10, volumeDb / 20f) * 100;
    _VolumeLimitSlider.Value = volumePercent;
    _VolumeLimitLabel.Text = $"Volume: {(int)volumePercent}%";
  }

  private void UpdateScreenModeDependentControls(bool isFullscreen) {
    UpdateBorderlessAvailability(isFullscreen);
    _resolutionOptionButton.Disabled = isFullscreen;
  }

  private void UpdateBorderlessAvailability(bool isFullscreen) {
    _borderlessCheckbox.Disabled = isFullscreen;
    _borderlessCheckbox.MouseDefaultCursorShape = isFullscreen ? CursorShape.Forbidden : CursorShape.PointingHand;
  }

  private void UpdateFpsLimitAvailability(bool vsyncEnabled) {
    int fpsLimit = (int)_fpsLimitSlider.Value;
    if (vsyncEnabled) {
      _fpsLimitSlider.Editable = false;
      _fpsLimitSlider.Visible = false;
      _fpsLimitSlider.MouseDefaultCursorShape = CursorShape.Forbidden;
      _fpsLimitLabel.Text = $"FPS Limit: V-Sync Enabled";
    }
    else {
      _fpsLimitSlider.Editable = true;
      _fpsLimitSlider.Visible = true;
      _fpsLimitSlider.MouseDefaultCursorShape = CursorShape.Hsize;
      var maxFps = Engine.MaxFps;
      _fpsLimitLabel.Text = $"FPS Limit: {(maxFps is 0 or 250 ? "Unlimited" : maxFps.ToString())}";
    }
  }

  private void OnResolutionOptionButtonPressed(long index) {
    if (_resolutionOptionButton.Disabled)
      return;

    var selectedResolution = index < Resolutions.Length
      ? Resolutions[index]
      : _resolutionOptionButton.GetItemText((int)index);

    var resolution = selectedResolution.Split('x');
    var width = int.Parse(resolution[0]);
    var height = int.Parse(resolution[1]);

    GetWindow().Size = new Vector2I(width, height);

    if (GetWindow().Mode == Window.ModeEnum.Windowed) {
      SettingsManager.CenterWindowOnCurrentMonitor();
    }

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
    }
    else {
      _resolutionOptionButton.Disabled = false;
      if (_resolutionOptionButton.GetItemCount() > 0) {
        var selectedIndex = _resolutionOptionButton.Selected;
        OnResolutionOptionButtonPressed(selectedIndex);
      }
    }

    UpdateScreenModeDependentControls(mode == Window.ModeEnum.Fullscreen);
    SettingsManager.SaveSettings();
  }

  private void OnBorderlessCheckboxToggled(bool buttonPressed) {
    if (GetWindow().Mode != Window.ModeEnum.Fullscreen) {
      GetWindow().Borderless = buttonPressed;
      SettingsManager.SaveSettings();
    }
  }

  private void OnVsyncCheckboxToggled(bool buttonPressed) {
    DisplayServer.WindowSetVsyncMode(buttonPressed
      ? DisplayServer.VSyncMode.Enabled
      : DisplayServer.VSyncMode.Disabled);
    UpdateFpsLimitAvailability(buttonPressed);
    SettingsManager.SaveSettings();
  }

  private void OnFpsLimiterSliderValueChanged(float value) {
    Engine.MaxFps = (int)value == 250 ? 0 : (int)value;
    _fpsLimitLabel.Text = $"FPS Limit: {(value is 0 or 250 ? "Unlimited" : value.ToString())}";
    SettingsManager.SaveSettings();
  }

  private void OnVolumeLimiterSliderValueChanged(float value) {
    float normalized = Math.Max(value / 100f, 0.0001f);
    float volumeDb = (float)Math.Log10(normalized) * 20f;

    AudioServer.SetBusVolumeDb(0, volumeDb);

    _VolumeLimitLabel.Text = $"Volume: {(int)value}%";
    SettingsManager.SaveSettings();
  }

  static SettingsMenu IWidgetFactory<SettingsMenu>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bc1qqk4div8dj").Instantiate<SettingsMenu>();
  }
}
