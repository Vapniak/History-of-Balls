namespace HOB;

using Godot;
using System;

public partial class TutorialPanel : Control {
  [Signal] public delegate void ClosedEventHandler();

  [Export] private TabContainer _tabContainer;
  [Export] private Label _tabsLabel;

  public override void _Ready() {
    base._Ready();

    UpdateTabsLabel();

    _tabContainer.TabChanged += (_) => UpdateTabsLabel();
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
    if (Visible && @event.IsActionPressed(BuiltinInputActions.UICancel)) {
      GetViewport().SetInputAsHandled();
      OnExitPressed();
    }
  }
  public void NextTab() {
    if (!_tabContainer.SelectNextAvailable()) {
      _tabContainer.CurrentTab = 0;
    }
  }

  public void PreviousTab() {
    if (!_tabContainer.SelectPreviousAvailable()) {
      _tabContainer.CurrentTab = _tabContainer.GetChildren().Count - 1;
    }
  }

  public void OnExitPressed() {
    EmitSignal(SignalName.Closed);
  }

  private void UpdateTabsLabel() {
    _tabsLabel.Text = $"{_tabContainer.CurrentTab + 1}/{_tabContainer.GetChildren().Count}";
  }
}
