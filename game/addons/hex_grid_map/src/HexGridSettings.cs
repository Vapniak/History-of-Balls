namespace HexGridMap;

using Godot;

[GlobalClass]
public partial class HexGridSettings : Resource {
  [Export] public GridShape GridShape { get; private set; }
  [Export] public HexLayout Layout { get; private set; } = new();
}
