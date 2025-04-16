namespace WidgetSystem;

using Godot;

public interface IWidget<out T> where T : Control {
  public bool CanBePopped();
}