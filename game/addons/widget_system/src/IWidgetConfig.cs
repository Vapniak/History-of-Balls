namespace WidgetSystem;

using System;
using Godot;

public interface IWidgetConfig<TWidget, TConfigurable> where TConfigurable : Control where TWidget : Widget {
  public TWidget Configure(Action<TConfigurable> config);
}
