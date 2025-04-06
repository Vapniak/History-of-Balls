namespace GameplayAbilitySystem;

using System;
using GameplayTags;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class GameplayEffectResource : Resource {
  [Export] public GameplayEffectPeriod? Period { get; private set; }
  [Export] public GameplayEffectDefinition? EffectDefinition { get; private set; }
  [Export] public GameplayEffectResource? ExpireEffect { get; private set; }
  [ExportGroup("Tags")]
  [Export] public TagContainer? AssetTags { get; private set; }
  [Export] public TagContainer? GrantedTags { get; private set; }
}