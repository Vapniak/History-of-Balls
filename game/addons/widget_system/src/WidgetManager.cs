namespace WidgetSystem;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WidgetManager : Node {
  private static WidgetManager _instance = default!;
  public static WidgetManager Instance => _instance ?? throw new InvalidOperationException("WidgetManager not initialized");

  public int WidgetStackCount => _widgetStack.Count;
  private readonly Stack<Widget> _widgetStack = new();
  private CanvasLayer _widgetLayer = default!;

  public Widget? CurrentWidget => _widgetStack.Count > 0 ? _widgetStack.Peek() : null;
  public event Action<Widget>? WidgetPushed;
  public event Action<Widget>? WidgetPopped;

  public override void _EnterTree() {
    if (_instance != null) {
      QueueFree();
      return;
    }
    _instance = this;

    _widgetLayer = new CanvasLayer { };
    AddChild(_widgetLayer);
  }

  public override void _ExitTree() {
    if (_instance == this) {
      _instance = null;
    }
  }

  public void PushWidget<T>(T widget, Action<T>? configure = null, bool hidePrevious = true) where T : Widget {
    if (hidePrevious && IsInstanceValid(CurrentWidget) && CurrentWidget != null) {
      CurrentWidget.Hide();
      CurrentWidget.ProcessMode = ProcessModeEnum.Disabled;
    }

    widget.ProcessMode = ProcessModeEnum.Inherit;
    _widgetLayer.AddChild(widget);
    _widgetStack.Push(widget);

    configure?.Invoke(widget);

    widget.Show();
    WidgetPushed?.Invoke(widget);
  }

  public void PushWidget<T>(Action<T>? configure = null, bool hidePrevious = true) where T : Widget, IWidgetFactory<T> {
    var widget = T.CreateWidget();

    PushWidget(widget, configure, hidePrevious);
  }

  public void PopWidget(bool showPrevious = true) {
    if (_widgetStack.Count == 0) {
      return;
    }

    var widget = _widgetStack.Pop();
    widget.QueueFree();

    if (showPrevious && IsInstanceValid(CurrentWidget) && CurrentWidget != null) {
      CurrentWidget.Show();
      CurrentWidget.ProcessMode = ProcessModeEnum.Inherit;
    }

    WidgetPopped?.Invoke(widget);
  }

  // public void ClearToWidget<T>(T widget) where T : Widget {
  //   while (_widgetStack.Count > 0 && CurrentWidget != widget) {
  //     PopWidget(false);
  //   }

  //   if (CurrentWidget != widget) {
  //     PushWidget(widget);
  //   }
  // }

  // public void PushNewWidget<T>(PackedScene scene, bool hidePrevious = true) where T : Widget {
  //   PushWidget(scene.Instantiate<T>(), hidePrevious);
  // }

  public void PopAllWidgets() {
    while (_widgetStack.Count > 0) {
      PopWidget(false);
    }
  }

  public override void _Input(InputEvent @event) {
    if (@event.IsActionPressed("ui_cancel")) {
      if (CurrentWidget != null && CurrentWidget.CanBePopped()) {
        PopWidget();
        GetViewport().SetInputAsHandled();
      }
    }
  }
}
