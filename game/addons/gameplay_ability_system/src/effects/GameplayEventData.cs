namespace GameplayAbilitySystem;

using Godot;

public sealed partial class GameplayEventData : RefCounted {
  public required object Activator;
  public GameplayAbilityTargetData? TargetData;
}