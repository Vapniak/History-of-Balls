namespace HOB;

using Godot;

[GlobalClass, Tool]
public partial class Hex : Resource {
  [Export] public int Col;
  [Export] public int Row;
  [Export] public Color Color;
  [Export] public int ObjectId;
}
