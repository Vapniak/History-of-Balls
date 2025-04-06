namespace HOB;

using Godot;

[GlobalClass]
public partial class AIProfile : Resource {
  [Export] public string ProfileName = "AI Profile";

  [ExportGroup("Behavior")]
  [Export(PropertyHint.Range, "0.1,1")] public float Agressiveness = 0.5f;
  [Export(PropertyHint.Range, "0.1,1")] public float Expansiveness = 0.5f;
}