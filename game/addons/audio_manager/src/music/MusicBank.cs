namespace AudioManager;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MusicBank : Resource {
  [Export] public string Label { get; private set; } = default!;
  [Export] public string Bus { get; private set; } = default!;
  [Export] public Array<MusicTrack> Tracks { get; private set; } = default!;

}
