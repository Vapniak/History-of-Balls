namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class SliderLabel : Label {
  [Export] public Slider Slider { get; private set; } = default!;

  public override void _Ready() {
    Text = string.Format("{0:0.0}", Slider.Value);

    Slider.ValueChanged += (value) => Text = string.Format("{0:0.0}", Slider.Value);
  }
}
