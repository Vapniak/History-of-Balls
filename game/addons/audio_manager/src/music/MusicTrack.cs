namespace AudioManager;

using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MusicTrack : Resource {
  [Export] public string Name { get; set; } = default!;
  [Export] public string Bus { get; set; } = default!;
  [Export] public Array<MusicStem> Stems { get; set; } = default!;
}