namespace HOB;

using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class ObjectiveWidget : HOBWidget, IWidgetFactory<ObjectiveWidget>, IWidgetConfig<ObjectiveWidget, RichTextLabel> {
  [Export] public RichTextLabel Label { get; private set; } = default!;

  public static ObjectiveWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://bqprlrd2o4jr2").Instantiate<ObjectiveWidget>().Configure(label => label.Text = "");
  }

  public ObjectiveWidget Configure(Action<RichTextLabel> config) {
    config.Invoke(Label);
    return this;
  }
}
