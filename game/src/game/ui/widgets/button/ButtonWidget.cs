namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class ButtonWidget : HOBWidget, IWidgetConfig<ButtonWidget, BaseButton> {
  [Export] public BaseButton Button { get; private set; } = default!;

  public override void _Ready() {
    FocusEntered += Button.GrabFocus;
  }

  public ButtonWidget Configure(Action<BaseButton> config) {
    config.Invoke(Button);
    return this;
  }
}
