namespace HOB;

using System.Diagnostics;
using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;

public partial class HOBGameplayAbilitySystem : GameplayAbilitySystem {
  public override void _Ready() {
    base._Ready();

    var events = GameInstance.GetGameMode<HOBGameMode>().GetMatchEvents();


    events.MatchEvent += OnMatchEvent;
  }

  protected override void Dispose(bool disposing) {
    base.Dispose(disposing);


    var events = GameInstance.GetGameMode<HOBGameMode>().GetMatchEvents();
    events.MatchEvent -= OnMatchEvent;
  }

  private void OnMatchEvent(Tag tag) {
    SendGameplayEvent(tag, null);

    var turnAware = GetOwnerOrNull<ITurnAware>();
    Debug.Assert(turnAware != null, "Owner has to be turn aware");

    if (turnAware != null) {
      Tick(new TurnTickContext(tag, turnAware.IsCurrentTurn()));
    }
  }
}
