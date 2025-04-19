namespace HOB;

using Godot;
using WidgetSystem;

public partial class FPSWidget : Widget, IWidgetFactory<FPSWidget> {
  [Export] private Label Label { get; set; } = default!;
  public static FPSWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://c5d5h1jxh7gwp").Instantiate<FPSWidget>();
  }

  public override void _Process(double delta) {
    base._Process(delta);

    Label.Text = ((int)Engine.GetFramesPerSecond()).ToString();
  }
}
