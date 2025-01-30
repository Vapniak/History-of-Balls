namespace HOB;

using GameplayFramework;

public interface IMatchController : IController {
  public bool IsOwnTurn();

  public new IMatchGameState GetGameState();
}
