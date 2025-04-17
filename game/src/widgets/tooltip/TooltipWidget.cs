namespace HOB;



using Godot;
using System;
using WidgetSystem;

public partial class TooltipWidget : Widget, IWidgetFactory<TooltipWidget>, IWidgetConfig<TooltipWidget, RichTextLabel> {
  [Export] private RichTextLabel Label { get; set; } = default!;


  public static TooltipWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://cafnytwiyv201").Instantiate<TooltipWidget>().Configure(label => label.Text = "");
  }

  public TooltipWidget Configure(Action<RichTextLabel> config) {
    config.Invoke(Label);
    return this;
  }
}
