namespace GameplayAbilitySystem;

using GameplayTags;
using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public abstract partial class GameplayAbilityResource : Resource {
  [Export] public string AbilityName { get; private set; } = "Ability";
  [Export] public GameplayEffectResource? CooldownGameplayEffect { get; private set; }
  [Export] public GameplayEffectResource? CostGameplayEffect { get; private set; }
  [Export] public bool ActivateOnGranted { get; private set; } = false;

  [Export] public Array<GameplayAbilityTriggerData>? AbilityTriggers { get; private set; }

  [ExportGroup("Tags")]
  [Export] public TagContainer? AbilityTags { get; private set; }
  //  [Export] public TagContainer? CancelAbilitiesWithTags { get; private set; }
  [Export] public TagContainer? ActivationBlockedTags { get; private set; }
  // [Export] public TagContainer? SourceRequiredTags { get; private set; }

  public abstract GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem);
}
