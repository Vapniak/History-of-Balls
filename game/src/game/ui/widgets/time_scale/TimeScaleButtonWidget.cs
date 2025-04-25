namespace HOB;

using Godot;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class TimeScaleButtonWidget : ButtonWidget {
  [Export] private Array<Texture2D> SpeedTextures { get; set; } = new();
  public override void _Process(double delta) {
    var index = (int)Engine.TimeScale - 1;
    var icon = SpeedTextures.ElementAtOrDefault(index);
    if (icon != null) {
      Button.Icon = icon;
    }
    //    Configure(label => label.Text =
    //string.Concat(Enumerable.Repeat(">", (int)Engine.TimeScale)));
  }
}
