namespace HOB;

using Godot;
using System;
using WidgetSystem;

public partial class TooltipWidget : HOBWidget, IWidgetFactory<TooltipWidget>, IWidgetConfig<TooltipWidget, RichTextLabel> {
  [Export] public RichTextLabel Label { get; private set; } = default!;


  public static TooltipWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://cafnytwiyv201").Instantiate<TooltipWidget>().Configure(label => label.Text = "");
  }

  public TooltipWidget Configure(Action<RichTextLabel> config) {
    config.Invoke(Label);
    return this;
  }
}
