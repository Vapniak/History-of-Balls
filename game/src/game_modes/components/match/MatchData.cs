namespace HOB;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MatchData : Resource {
  [Export] public MapData? Map { get; private set; }
  [Export] public Array<MatchPlayerSpawnData> PlayerSpawnDatas { get; private set; } = new();
}
