namespace GameplayAbilitySystem;

using Godot;

[GlobalClass]
public partial class GameplayEffectResource : Resource {
  [Export] public GameplayEffectPeriod? Period { get; private set; }
  [Export] public GameplayEffectDefinition? EffectDefinition { get; private set; }
}