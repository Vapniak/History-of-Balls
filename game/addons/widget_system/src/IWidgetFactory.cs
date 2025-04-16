namespace WidgetSystem;

using Godot;

public interface IWidgetFactory<T> where T : Control {
  public static abstract T CreateWidget();
}
