namespace HOB;

using System;

public interface IMatchEvents {
  public event Action TurnStarted;
  public event Action TurnChanged;
  public event Action TurnEnded;
  public event Action RoundStarted;

  public event Action<IMatchController> GameEnded;

  public bool IsCurrentTurn(IMatchController controller);
}
