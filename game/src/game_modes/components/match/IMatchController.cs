namespace HOB;

using GameplayFramework;

public interface IMatchController : IController {
  public bool OwnTurn();
}
