namespace HOB;

using System;
using GameplayFramework;

public interface IMatchController : IController {
  public event Action EndTurnEvent;

  public Team Team { get; set; }

  public void OnGameStarted();

  public bool IsCurrentTurn();
  public void OwnTurnStarted();
  public void OwnTurnEnded();

  public new IMatchGameState GetGameState();

  public new IMatchPlayerState GetPlayerState();
}
