namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;
using System;
using WidgetSystem;

[GlobalClass]
public partial class SelectMissionWidget : HOBWidget, IWidgetFactory<SelectMissionWidget> {
  [Export] private Control MissionList { get; set; } = default!;

  private Array<MissionData> Missions { get; set; } = default!;

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

    foreach (var missionData in Missions) {
      var missionWidget = MissionWidget.CreateWidget();
      missionWidget.Button.Pressed += () => GameInstance.GetGameMode<MainMenuGameMode>()?.StartMission(missionData);
      missionWidget.BindTo(missionData);
      MissionList.AddChild(missionWidget);
    }
  }
}
