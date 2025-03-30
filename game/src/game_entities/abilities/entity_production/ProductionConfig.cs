namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;

[GlobalClass]
public partial class ProductionConfig : Resource {
  [Export] public EntityData? Entity { get; private set; }
  [Export] public GameplayEffectResource? CostEffect { get; private set; }
  [Export] public uint ProductionTime { get; private set; }
}