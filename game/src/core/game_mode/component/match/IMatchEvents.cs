namespace HOB;

using System;
using GameplayTags;

public interface IMatchEvents {

  public event Action<Tag> MatchEvent;
  public bool IsCurrentTurn(IMatchController controller);
}
