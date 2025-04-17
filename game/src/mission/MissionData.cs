namespace HOB;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MissionData : Resource {
  [Export] public string MissionName { get; private set; } = "";
  [Export(PropertyHint.MultilineText)] public string Description { get; private set; } = "";
  [Export] public MapData Map { get; private set; } = default!;
  [Export] public Array<MatchPlayerSpawnData> PlayerSpawnDatas { get; private set; } = new();
}
