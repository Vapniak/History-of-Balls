namespace GameplayAbilitySystem;

using GameplayTags;
using Godot;

public sealed partial class GameplayEventData : RefCounted {
  public required object Activator;
  public Tag? EventTag { get; set; }
  public GameplayAbilityTargetData? TargetData;
}