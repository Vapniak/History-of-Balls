namespace HOB;

using Godot;

[GlobalClass]
public partial class Country : Resource {
  [Export] public string Name { get; private set; } = "Country";
  [Export] public Color Color { get; private set; }
  [Export] public Texture2D? Flag { get; private set; }
  [Export] public ResourceType? PrimaryResource { get; private set; }
  [Export] public ResourceType? SecondaryResource { get; private set; }
}
