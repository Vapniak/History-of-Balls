namespace HOB;

using Godot;
using HOB;
using System;
using WidgetSystem;

[GlobalClass]
public partial class LabelButtonWidget : ButtonWidget, IWidgetConfig<LabelButtonWidget, RichTextLabel> {
  [Export] public RichTextLabel Label { get; private set; } = default!;

  public LabelButtonWidget Configure(Action<RichTextLabel> config) {
    config.Invoke(Label);
    return this;
  }
}
