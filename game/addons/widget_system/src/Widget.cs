namespace WidgetSystem;

using System;
using Godot;

[GlobalClass]
public partial class Widget : Control, IWidget<Widget> {
  protected WidgetManager WidgetManager => WidgetManager.Instance;

  public static Widget Create() {
    return new();
  }

  public void CloseWidget() {
    WidgetManager.PopWidget();
  }

  public virtual bool CanBePopped() => WidgetManager.WidgetStackCount > 1;
}
