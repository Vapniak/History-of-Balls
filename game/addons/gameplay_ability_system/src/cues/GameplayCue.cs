namespace GameplayAbilitySystem;

using GameplayTags;
using Godot;

[GlobalClass]
public abstract partial class GameplayCue : Node {
  /// <summary>
  /// Called when duration or infinite effect is activated
  /// </summary>
  /// <param name="parameters"></param>

  [Export] public Tag CueTag { get; set; } = default!;
  public virtual void OnActive(GameplayCueParameters parameters) {

  }

  public virtual void OnExecute(GameplayCueParameters parameters) {

  }

  public virtual void OnRemove(GameplayCueParameters parameters) {

  }

  public abstract bool HandlesEvent(GameplayCueEventType @event);

  public void HandleGameplayCue(GameplayCueEventType @event, GameplayCueParameters parameters) {
    if (HandlesEvent(@event)) {
      switch (@event) {
        case GameplayCueEventType.OnActive:
          OnActive(parameters);
          break;
        case GameplayCueEventType.Executed:
          OnExecute(parameters);
          break;
        case GameplayCueEventType.Removed:
          OnRemove(parameters);
          break;
        default:
          break;
      }
    }
  }
}

public enum GameplayCueEventType {
  OnActive,
  Executed,
  Removed
}
