namespace WidgetSystem;

using System;
using System.Threading.Tasks;
using Godot;

public interface IAsyncWidgetFactory<T> : IWidgetFactory<T> where T : Control {
  public static new abstract Task<T> CreateWidget();
}