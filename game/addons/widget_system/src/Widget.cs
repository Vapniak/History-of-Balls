namespace WidgetSystem;

using System;
using Godot;

[GlobalClass]
public partial class Widget : Control, IWidget<Widget>, IWidgetFactory<Widget> {
  protected WidgetManager WidgetManager => WidgetManager.Instance;

  static Widget IWidgetFactory<Widget>.CreateWidget() => new();

  public void PopWidget() {
    WidgetManager.PopWidget();
  }
  public virtual bool CanBePopped() => WidgetManager.WidgetStackCount > 1;
}
