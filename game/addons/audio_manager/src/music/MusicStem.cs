namespace AudioManager;

using Godot;

[GlobalClass]
public partial class MusicStem : Resource {
  [Export] public string Name { get; set; } = "";
  [Export] public bool Enabled { get; set; } = true;
  [Export(PropertyHint.Range, "0,1")] public float Volume { get; set; } = 1;
  [Export] public AudioStream Stream { get; set; } = new();

  public MusicStem(string name, bool enabled, float volume, AudioStream audioStream) {
    Name = name;
    Enabled = enabled;
    Volume = volume;
    Stream = audioStream;
  }

  public MusicStem() { }
}