namespace HOB;

using System;
using GameplayFramework;

public interface IMatchController : IController {
  public event Action EndTurnEvent;

  public bool IsCurrentTurn();

  public new IMatchGameState GetGameState();
}
