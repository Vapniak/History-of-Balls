namespace HOB;

using Godot;
using System;
using System.Linq;
using WidgetSystem;

[GlobalClass]
public partial class TimeScaleButtonWidget : LabelButtonWidget {
  public override void _Process(double delta) {
    Configure(label => label.Text =
      string.Concat(Enumerable.Repeat(">", (int)Engine.TimeScale)));
  }
}
