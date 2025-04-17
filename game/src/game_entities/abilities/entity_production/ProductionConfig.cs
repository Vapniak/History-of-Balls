namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using Godot;
using HOB.GameEntity;

[GlobalClass, Tool]
public partial class ProductionConfig : Resource {
  [Export] public EntityData Entity { get; private set; } = default!;
  [Export] public GameplayEffectResource CostEffect { get; private set; } = default!;
  [Export] public uint ProductionTime { get; private set; }
}