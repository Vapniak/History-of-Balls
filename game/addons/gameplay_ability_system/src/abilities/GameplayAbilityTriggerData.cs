namespace GameplayAbilitySystem;

using System;
using GameplayTags;
using Godot;

[GlobalClass]
public partial class GameplayAbilityTriggerData : Resource {
  [Export] public AbilityTriggerSourceType TriggerSource { get; private set; }
  [Export] public Tag? TriggerTag { get; private set; }
}

[Flags]
public enum AbilityTriggerSourceType : int {
  /// <summary>
  /// Triggered when trigger tags is in event
  /// </summary>
  GameplayEvent = 1,
  /// <summary>
  /// Triggered when the trigger tag gets added to owner of ability
  /// </summary>
  OwnedTagAdded = 2,
}