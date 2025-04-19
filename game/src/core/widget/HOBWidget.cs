namespace HOB;

using Godot;
using System;
using WidgetSystem;

public abstract partial class HOBWidget : Widget {
  public override GodotObject _MakeCustomTooltip(string forText) {
    return TooltipWidget.CreateWidget().Configure(label => label.Text = forText);
  }
}
