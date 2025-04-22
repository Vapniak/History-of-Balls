namespace HOB;

using GameplayAbilitySystem;
using Godot;

[GlobalClass]
public partial class AttributeIcon : Resource {
  [Export] public GameplayAttribute Attribute { get; private set; } = new();
  [Export] public Texture2D Icon { get; private set; } = new();
  [Export] public Color Color { get; private set; }

  public AttributeIcon() {

  }

  public AttributeIcon(GameplayAttribute attribute, Texture2D icon) {
    Attribute = attribute;
    Icon = icon;
  }
}