namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class AttributeModifier : Resource {
  [Export] public float Add { get; set; }
  [Export] public float Multiply { get; set; }
  [Export] public float Override { get; set; }
}