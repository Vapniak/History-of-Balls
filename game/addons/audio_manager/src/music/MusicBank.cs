namespace AudioManager;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MusicBank : Resource {
  [Export] public string Label { get; private set; } = "";
  [Export] public string Bus { get; private set; } = "";
  [Export] public Node.ProcessModeEnum Mode { get; private set; }
  [Export] public Array<MusicTrack> Tracks { get; private set; } = new();

}
