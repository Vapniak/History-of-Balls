namespace WidgetSystem;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class WidgetManager : Node {
  private static WidgetManager _instance = default!;
  public static WidgetManager Instance => _instance ?? throw new InvalidOperationException("WidgetManager not initialized");

  public event Action<Widget>? WidgetPushed;
  public event Action<Widget>? WidgetPopped;

  [Export] private CanvasLayer WidgetLayer { get; set; } = default!;

  public int WidgetStackCount => _widgetStack.Count;
  public Widget? CurrentWidget => _widgetStack.Count > 0 ? _widgetStack.Peek() : null;

  private readonly Stack<Widget> _widgetStack = new();

  public override void _EnterTree() {
    if (_instance != null) {
      QueueFree();
      return;
    }
    _instance = this;
  }

  public override void _ExitTree() {
    if (_instance == this) {
      _instance = null;
    }
  }

  public void PushWidget<T>(T widget, Action<T>? configure = null, bool hidePrevious = true) where T : Widget {
    if (hidePrevious) {
      HideCurrentWidget();
    }

    AddWidget(widget, configure);
  }

  public void PushWidget<T>(Action<T>? configure = null, bool hidePrevious = true) where T : Widget, IWidgetFactory<T> {
    var widget = T.CreateWidget();

    PushWidget(widget, configure, hidePrevious);
  }

  public async Task PushWidgetAsync<T>(Action<T>? configure = null, bool hidePrevious = true) where T : Widget, IAsyncWidgetFactory<T> {
    if (hidePrevious) {
      HideCurrentWidget();
    }
    // TODO: show loading indicator

    var widget = await T.CreateWidget();

    AddWidget(widget, configure);
  }

  public void PopWidget(bool showPrevious = true) {
    if (_widgetStack.Count == 0) {
      return;
    }

    var widget = _widgetStack.Pop();
    widget.OnPopped();
    widget.QueueFree();

    if (showPrevious && IsInstanceValid(CurrentWidget) && CurrentWidget != null) {
      CurrentWidget.Show();
      CurrentWidget.CallDeferred(Control.MethodName.GrabFocus);
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

  private void HideCurrentWidget() {
    if (IsInstanceValid(CurrentWidget) && CurrentWidget != null) {
      CurrentWidget.Hide();
      CurrentWidget.ProcessMode = ProcessModeEnum.Disabled;
    }
  }

  private void AddWidget<T>(T widget, Action<T>? configure = null) where T : Widget {
    _widgetStack.Push(widget);

    widget.ProcessMode = ProcessModeEnum.Inherit;
    WidgetLayer.AddChild(widget);

    configure?.Invoke(widget);

    widget.Show();
    widget.CallDeferred(Control.MethodName.GrabFocus);
    WidgetPushed?.Invoke(widget);
  }
}
