namespace GameplayAbilitySystem;

using System;
using GameplayTags;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GameplayEffectResource : Resource {
  [Export] public GameplayEffectPeriod? Period { get; private set; }
  [Export] public GameplayEffectDefinition? EffectDefinition { get; private set; }
  [ExportCategory("Tags")]
  [Export] public TagContainer? EffectTags { get; private set; }
  [Export] public TagContainer? GrantedTags { get; private set; }
}