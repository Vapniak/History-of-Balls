namespace HOB;

using GameplayFramework;
using Godot;
using System;

public partial class HOBGameInstance : GameInstance {
  public static void StartMission(MissionData missionData) {
    var loadingScreenScene = ResourceLoader.Load<PackedScene>("uid://4np7vinx0jpp");
    _ = GetWorld().OpenLevelThreaded("mission_level", loadingScreenScene, missionData.GetGameModeConfig());
  }
}
