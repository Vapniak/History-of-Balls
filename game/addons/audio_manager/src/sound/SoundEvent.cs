namespace AudioManager;

using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SoundEvent : Resource {
  [Export] public string Name { get; private set; } = default!;
  [Export] public string Bus { get; private set; } = default!;
  [Export(PropertyHint.Range, "-80,6,0.1,suffix:dB")] public float Volume { get; private set; }
  [Export] public float Pitch { get; private set; } = 1;
  [Export] public Array<AudioStream> Streams { get; private set; } = default!;
}