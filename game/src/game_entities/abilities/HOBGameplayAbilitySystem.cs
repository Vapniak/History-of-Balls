namespace HOB;

using System.Diagnostics;
using GameplayAbilitySystem;
using GameplayFramework;
public partial class HOBGameplayAbilitySystem : GameplayAbilitySystem {
  public override void _Ready() {
    base._Ready();

    var events = GameInstance.GetGameMode<HOBGameMode>().GetMatchEvents();

    var turnAware = GetOwnerOrNull<ITurnAware>();
    Debug.Assert(turnAware != null, "Owner has to be turn aware");
    if (turnAware != null) {
      events.TurnStarted += OnTurnStarted;
      events.TurnEnded += OnTurnEnded;
    }
  }

  protected override void Dispose(bool disposing) {
    base.Dispose(disposing);


    var events = GameInstance.GetGameMode<HOBGameMode>().GetMatchEvents();
    events.TurnStarted -= OnTurnStarted;
    events.TurnEnded -= OnTurnEnded;
  }

  private void OnTurnStarted() {
    Tick(new TurnTickContext(TurnPhase.Start, GetOwner<ITurnAware>().IsCurrentTurn()));
  }

  private void OnTurnEnded() {
    Tick(new TurnTickContext(TurnPhase.End, GetOwner<ITurnAware>().IsCurrentTurn()));
  }
}
