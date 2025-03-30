namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;

[GlobalClass]
public partial class ProductionConfig : Resource {
  [Export] public Tag? EntityTag { get; private set; }
  [Export] public GameplayEffectResource? CostEffect { get; private set; }
  [Export] public uint ProductionTime { get; private set; }
}