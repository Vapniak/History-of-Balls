namespace WidgetSystem;

using Godot;

public interface IWidget<T> where T : Control, IWidget<T> {
  public static abstract T Create();
}