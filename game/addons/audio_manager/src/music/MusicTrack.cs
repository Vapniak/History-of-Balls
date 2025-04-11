namespace AudioManager;

using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MusicTrack : Resource {
  [Export] public string Name { get; set; } = "";
  [Export] public string Bus { get; set; } = "";
  [Export] public Array<MusicStem> Stems { get; set; } = new();
}