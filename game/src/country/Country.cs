namespace HOB;

using Godot;

[GlobalClass]
public partial class Country : Resource {
  [Export] public string Name { get; private set; }
  // TODO: add flag or icon
  [Export] public Color Color { get; private set; }
  [Export] public Texture2D Flag { get; private set; }

  public IMatchController Leader { get; set; }
}
