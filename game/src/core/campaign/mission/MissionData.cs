namespace HOB;

using GameplayFramework;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MissionData : Resource {
  [Export] public string MissionName { get; private set; } = "";
  [Export(PropertyHint.MultilineText)] public string Description { get; private set; } = "";
  [Export(PropertyHint.MultilineText)] public string ObjectivesText { get; private set; } = "";
  [Export] public string MusicTrackName { get; private set; } = "";

  [Export] public bool Locked { get; private set; } = true;
  [Export] public MapData Map { get; private set; } = default!;
  [Export] public Array<MatchPlayerSpawnData> PlayerSpawnDatas { get; private set; } = new();

  [Export] private HOBGameModeConfig GameModeConfig { get; set; } = default!;

  public HOBGameModeConfig GetGameModeConfig() {
    GameModeConfig.Mission = this;
    return GameModeConfig;
  }
}
