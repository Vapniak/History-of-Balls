namespace GameplayAbilitySystem;

using GameplayTags;
using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public abstract partial class GameplayAbilityResource : Resource {
  [Export] public string AbilityName { get; protected set; } = "Ability";
  [Export] public GameplayEffectResource? CooldownGameplayEffect { get; protected set; }
  [Export] public GameplayEffectResource? CostGameplayEffect { get; protected set; }
  [Export] public bool ActivateOnGranted { get; protected set; } = false;
  [Export] public bool RemoveOnEnd { get; protected set; } = false;

  [Export] public Array<GameplayAbilityTriggerData>? AbilityTriggers { get; protected set; }

  [ExportGroup("Tags")]
  [Export] public TagContainer? AbilityTags { get; protected set; }
  [Export] public TagContainer? CancelAbilitiesWithTags { get; private set; }
  [Export] public TagContainer? ActivationBlockedTags { get; protected set; }
  [Export] public TagContainer? ActivationRequiredTags { get; protected set; }
  // [Export] public TagContainer? SourceRequiredTags { get; private set; }

  public abstract GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem);
}
