namespace HOB;

using Godot;
using System;

[GlobalClass]
public partial class SliderLabel : Label {
  [Export] public Slider Slider { get; private set; } = default!;

  public override void _Ready() {
    Text = Slider.Value.ToString();

    Slider.ValueChanged += (value) => Text = value.ToString();
  }
}
