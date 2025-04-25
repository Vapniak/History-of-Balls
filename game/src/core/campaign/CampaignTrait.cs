namespace HOB;

using Godot;

[GlobalClass]
public partial class CampaignTrait : Resource {
  [Export] public string Name { get; private set; } = "";
  [Export] public float Percent { get; private set; } = 0;
}
