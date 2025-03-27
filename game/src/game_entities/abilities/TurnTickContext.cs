namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;

public class TurnTickContext : ITickContext {
  public Tag Event { get; private set; }
  public bool OwnTurn { get; private set; }
  public TurnTickContext(Tag @event, bool ownTurn) {
    Event = @event;
    OwnTurn = ownTurn;
  }
}
