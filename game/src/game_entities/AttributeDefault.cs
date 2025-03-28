namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class AttributeDefault : Resource {
  [Export] public GameplayAttribute? Attribute { get; private set; }
  [Export] public float DefaultValue { get; private set; }

  public AttributeDefault() { }
  public AttributeDefault(GameplayAttribute attribute, float value) {
    Attribute = attribute;
    DefaultValue = value;
  }
}