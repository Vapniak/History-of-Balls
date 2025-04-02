namespace HOB;

using System.Diagnostics;
using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;

public partial class HOBGameplayAbilitySystem : GameplayAbilitySystem {
  public override void _Ready() {
    base._Ready();

    var events = GameInstance.GetGameMode<HOBGameMode>()?.GetMatchEvents();

    if (events != null) {
      events.MatchEvent += OnMatchEvent;
    }
  }

  protected override void Dispose(bool disposing) {
    base.Dispose(disposing);


    var gm = GameInstance.GetGameMode<HOBGameMode>();
    if (IsInstanceValid(gm) && gm != null) {
      var events = gm.GetMatchEvents();
      if (events != null) {
        events.MatchEvent -= OnMatchEvent;
      }
    }
  }

  private void OnMatchEvent(Tag tag) {
    var turnAware = GetOwnerOrNull<ITurnAware>();
    Debug.Assert(turnAware != null, "Owner has to be turn aware");

    if (turnAware != null) {
      if (tag == TagManager.GetTag(HOBTags.EventTurnStarted)) {
        if (turnAware.IsCurrentTurn()) {
          OwnedTags.AddTag(TagManager.GetTag(HOBTags.StateCurrentTurn));
        }
      }
      else if (tag == TagManager.GetTag(HOBTags.EventTurnEnded)) {
        if (turnAware.IsCurrentTurn()) {
          OwnedTags.RemoveTag(TagManager.GetTag(HOBTags.StateCurrentTurn));
        }
      }
    }

    SendGameplayEvent(tag, null);

    if (turnAware != null) {
      Tick(new TurnTickContext(tag, turnAware.IsCurrentTurn()));
    }
  }
}
