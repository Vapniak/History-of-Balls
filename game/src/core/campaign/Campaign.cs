namespace HOB;

using Godot;
using Godot.Collections;
using System;

[GlobalClass, Tool]
public partial class Campaign : Resource {
  [Export] public string Number { get; private set; } = "";
  [Export] public string Name { get; private set; } = "";
  [Export] public Array<CampaignTrait> Traits { get; private set; } = new();
  [Export] public Array<MissionData> Missions { get; private set; } = default!;
  [Export] public Texture2D Image { get; private set; } = default!;
}
