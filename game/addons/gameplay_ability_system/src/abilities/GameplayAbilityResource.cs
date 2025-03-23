namespace GameplayAbilitySystem;

using GameplayTags;
using Godot;
using System;

[GlobalClass]
public abstract partial class GameplayAbilityResource : Resource {
  [Export] public string AbilityName { get; private set; } = "Ability";
  [Export] public GameplayEffectResource? CooldownGameplayEffect { get; private set; }
  [Export] public GameplayEffectResource? CostGameplayEffect { get; private set; }

  public abstract GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem);
}
