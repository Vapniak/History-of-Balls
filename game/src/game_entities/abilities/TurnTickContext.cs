namespace HOB;

using GameplayAbilitySystem;

public class TurnTickContext : ITickContext {
  public TurnPhase TurnPhase { get; private set; }
  public bool OwnTurn { get; private set; }
  public TurnTickContext(TurnPhase phase, bool ownTurn) {
    TurnPhase = phase;
    OwnTurn = ownTurn;
  }
}
