namespace HOB;

using GameplayFramework;
using Godot;
using System;
using WidgetSystem;

[GlobalClass]
public partial class TutorialMenuWidget : HOBWidget, IWidgetFactory<TutorialMenuWidget> {
  [Export] public MissionData TutorialMission { get; private set; } = default!;

  [Export] private TabContainer _tabContainer;
  [Export] private Label _tabsLabel;

  public override void _Ready() {
    base._Ready();

    UpdateTabsLabel();

    _tabContainer.TabChanged += (_) => UpdateTabsLabel();
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

  private void StartTutorialMission() {
    HOBGameInstance.StartMission(TutorialMission);
  }

  private void UpdateTabsLabel() {
    _tabsLabel.Text = $"{_tabContainer.CurrentTab + 1}/{_tabContainer.GetChildren().Count}";
  }


  static TutorialMenuWidget IWidgetFactory<TutorialMenuWidget>.CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://stlrfv5fg27j").Instantiate<TutorialMenuWidget>();
  }
}
