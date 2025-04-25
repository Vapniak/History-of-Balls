namespace HOB;

using System;
using System.ComponentModel;
using Godot;
using WidgetSystem;

[Tool]
public abstract partial class BaseButtonWidget<TSelf, T> : HOBWidget, IWidgetConfig<BaseButtonWidget<TSelf, T>, T> where T : BaseButton {
  [Signal] public delegate void StateFontColorChangedEventHandler(Color color);

  [Export] private Control? ControlToModulate { get; set; }
  [Export] private bool ModulateOnButtonStateChange { get; set; } = true;
  public abstract T Button { get; protected set; }

  private bool _lastDisabled;


  public BaseButtonWidget<TSelf, T> Configure(Action<T> config) {
    config.Invoke(Button);
    return this;
  }

  public override void _EnterTree() {
    base._EnterTree();

    ControlToModulate ??= this;

    if (Engine.IsEditorHint()) {
      return;
    }

    if (!IsInstanceValid(Button)) {
      return;
    }

    Button.MouseEntered += UpdateColors;
    Button.MouseExited += UpdateColors;
    Button.ButtonDown += UpdateColors;
    Button.ButtonUp += UpdateColors;
    Button.Toggled += _ => UpdateColors();
  }


  public override void _Process(double delta) {
    base._Process(delta);

    if (Engine.IsEditorHint()) {
      return;
    }

    if (IsInstanceValid(Button) && _lastDisabled != Button.Disabled) {
      UpdateColors();
      _lastDisabled = Button.Disabled;
    }
  }

  private void UpdateColors() {
    var color = Colors.White;

    if (Button.Disabled) {
      color = Button.GetThemeColor("font_disabled_color");
    }
    else if (Button.ButtonPressed) {
      color = Button.GetThemeColor("font_pressed_color");
    }
    else if (Button.IsHovered()) {
      color = Button.GetThemeColor("font_hover_color");
    }
    else {
      color = Button.GetThemeColor("font_color");
    }

    foreach (var child in GetChildren()) {
      if (child != Button && child is Control control) {
        ControlToModulate = control;
        break;
      }
    }

    if (ModulateOnButtonStateChange && ControlToModulate != null) {
      ControlToModulate.Modulate = color;
    }

    EmitSignal(SignalName.StateFontColorChanged, color);
  }
}