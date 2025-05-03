namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;
using System;
using System.Linq;
using WidgetSystem;

[GlobalClass]
public partial class SelectMissionWidget : HOBWidget, IWidgetFactory<SelectMissionWidget> {
  [Export] private Control MissionList { get; set; } = default!;

  [Export] private Label MissionName { get; set; } = default!;
  [Export] private FlagWidget Flag1 { get; set; } = default!;
  [Export] private Label Country1 { get; set; } = default!;

  [Export] private FlagWidget Flag2 { get; set; } = default!;
  [Export] private Label Country2 { get; set; } = default!;

  [Export] private Label DescriptionLabel { get; set; } = default!;
  [Export] private ButtonWidget StartButton { get; set; } = default!;


  private Array<MissionData> Missions { get; set; } = default!;

  private MissionData? SelectedMission { get; set; }

  public static SelectMissionWidget CreateWidget() {
    return ResourceLoader.Load<PackedScene>("uid://dyib3loosl782").Instantiate<SelectMissionWidget>();
  }

  public void BindTo(Array<MissionData> missions) {
    Missions = missions;
  }

  public override void _Ready() {
    base._Ready();

    foreach (var child in MissionList.GetChildren()) {
      child.QueueFree();
    }

    StartButton.Button.Pressed += () => {
      if (SelectedMission == null) {
        return;
      }

      HOBGameInstance.StartMission(SelectedMission);
    };

    foreach (var missionData in Missions) {
      var missionWidget = MissionWidget.CreateWidget();
      missionWidget.ButtonWidget.Button.Toggled += (_) => SelectMission(missionData);
      missionWidget.BindTo(missionData);
      MissionList.AddChild(missionWidget);
      if (Missions.First() == missionData) {
        missionWidget.ButtonWidget.Button.ButtonPressed = true;
      }
    }
  }

  private void SelectMission(MissionData mission) {
    SelectedMission = mission;
    MissionName.Text = mission.MissionName;

    var first = mission.PlayerSpawnDatas.ElementAt(0).Country;
    Flag1.SetFlag(first.Flag!);
    Country1.Text = first.Name;

    var second = mission.PlayerSpawnDatas.ElementAt(1).Country;
    Flag2.SetFlag(second.Flag!);
    Country2.Text = second.Name;


    DescriptionLabel.Text = mission.Description;
  }
}
