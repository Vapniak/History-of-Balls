namespace HOB;

using System;
using GameplayFramework;

public interface IMatchController : IController {
  public bool IsCurrentTurn() => this.GetGameMode<HOBGameMode>().IsCurrentTurn(this);

  public void OnGameStarted();
  public void OwnTurnStarted();
  public void OwnTurnEnded();

  public new IMatchPlayerState GetPlayerState();
}
