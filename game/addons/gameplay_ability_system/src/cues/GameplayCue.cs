namespace GameplayAbilitySystem;

using GameplayTags;
using Godot;

[GlobalClass]
public abstract partial class GameplayCue : Node {
  /// <summary>
  /// Called when duration or infinite effect is activated
  /// </summary>
  /// <param name="target"></param>
  /// <param name="parameters"></param>

  public abstract Tag CueTag { get; protected set; }
  public virtual void OnActive(Node target, GameplayCueParameters parameters) {

  }

  public virtual void OnExecute(Node target, GameplayCueParameters parameters) {

  }

  public virtual void OnRemove(Node target, GameplayCueParameters parameters) {

  }

  public abstract bool HandlesEvent(GameplayCueEventType @event);

  public void HandleGameplayCue(Node target, GameplayCueEventType @event, GameplayCueParameters parameters) {
    if (HandlesEvent(@event)) {
      switch (@event) {
        case GameplayCueEventType.OnActive:
          OnActive(target, parameters);
          break;
        case GameplayCueEventType.Executed:
          OnExecute(target, parameters);
          break;
        case GameplayCueEventType.Removed:
          OnRemove(target, parameters);
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
