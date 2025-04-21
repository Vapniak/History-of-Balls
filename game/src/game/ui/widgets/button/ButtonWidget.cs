namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class ButtonWidget : Widget, IWidgetConfig<ButtonWidget, BaseButton>, IWidgetConfig<ButtonWidget, RichTextLabel> {
  [Export] private BaseButton Button { get; set; } = default!;
  [Export] private RichTextLabel Label { get; set; } = default!;

  public ButtonWidget Configure(Action<BaseButton> config) {
    config.Invoke(Button);
    return this;
  }
  public ButtonWidget Configure(Action<RichTextLabel> config) {
    config.Invoke(Label);
    return this;
  }
}
