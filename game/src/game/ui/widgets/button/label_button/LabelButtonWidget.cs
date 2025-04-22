namespace HOB;

using Godot;
using HOB;
using System;
using WidgetSystem;

[GlobalClass]
public partial class LabelButtonWidget : ButtonWidget, IWidgetConfig<LabelButtonWidget, RichTextLabel>, IWidgetFactory<LabelButtonWidget> {
  [Export] public RichTextLabel Label { get; private set; } = default!;

  public static LabelButtonWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bucr6se6rejrr").Instantiate<LabelButtonWidget>();
  }

  public LabelButtonWidget Configure(Action<RichTextLabel> config) {
    config.Invoke(Label);
    return this;
  }
}
