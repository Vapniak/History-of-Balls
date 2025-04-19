namespace HOB;

using System;

public interface ITurnManagment {
  public event Action? TurnBlocked;
  public event Action? TurnUnblocked;

  public void AddBlockTurn();
  public void RemoveBlockTurn();

  public bool TryEndTurn(IMatchController controller);
}
