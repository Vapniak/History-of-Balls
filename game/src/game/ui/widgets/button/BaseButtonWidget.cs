namespace HOB;

using System;
using Godot;
using WidgetSystem;

public abstract partial class BaseButtonWidget<TSelf, T> : HOBWidget, IWidgetConfig<BaseButtonWidget<TSelf, T>, T> where T : BaseButton {
  public abstract T Button { get; protected set; }
  public BaseButtonWidget<TSelf, T> Configure(Action<T> config) {
    config.Invoke(Button);
    return this;
  }
}