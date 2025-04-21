namespace HOB;

using GameplayTags;
using Godot;

[GlobalClass]
public partial class EntityIcon : Resource {
  [Export] public Tag EntityType { get; private set; } = default!;
  [Export] public Texture2D Icon { get; private set; } = new();
}
