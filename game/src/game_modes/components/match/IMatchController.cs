namespace HOB;

using System;
using GameplayFramework;

public interface IMatchController : IController {
  public event Action EndTurnEvent;

  public bool IsCurrentTurn();
  public void OwnTurnStarted();
  public void OwnTurnEnded();

  public new IMatchGameState GetGameState();
}
