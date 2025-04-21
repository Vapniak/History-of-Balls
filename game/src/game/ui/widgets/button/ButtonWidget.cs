namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class ButtonWidget : BaseButtonWidget<Button>, IWidgetConfig<ButtonWidget, BaseButton> {
  [Export] public override Button Button { get; protected set; } = default!;

  public override void _Ready() {
    FocusEntered += Button.GrabFocus;
  }

  public ButtonWidget Configure(Action<BaseButton> config) {
    config.Invoke(Button);
    return this;
  }

  public override BaseButtonWidget<Button> Configure(Action<Button> config) => throw new NotImplementedException();
}
