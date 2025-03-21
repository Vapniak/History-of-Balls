namespace GameplayAbilitySystem;

using System;
using Godot;

[GlobalClass]
public partial class AttributeModifier : Resource {
  [Export] public float Add { get; set; }
  [Export] public float Multiply { get; set; }
  [Export] public float Override { get; set; }

  public AttributeModifier Combine(AttributeModifier other) {
    other.Add += Add;
    other.Multiply += Multiply;
    other.Override = Override;
    return other;
  }
}