namespace HOB;

using System;
using Godot;
using WidgetSystem;

public abstract partial class BaseButtonWidget<T> : HOBWidget, IWidgetConfig<BaseButtonWidget<T>, T> where T : BaseButton {
  public abstract T Button { get; protected set; }
  public abstract BaseButtonWidget<T> Configure(Action<T> config);
}