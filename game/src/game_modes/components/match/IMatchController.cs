namespace HOB;

using System;
using GameplayFramework;

public interface IMatchController : IController {
  public bool IsCurrentTurn() => GetGameMode().IsCurrentTurn(this);

  public void OnGameStarted();
  public void OwnTurnStarted();
  public void OwnTurnEnded();

  public new IMatchGameState GetGameState();

  public new HOBGameMode GetGameMode();
  public new IMatchPlayerState GetPlayerState();
}
