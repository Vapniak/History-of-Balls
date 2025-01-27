namespace HOB.Player;

using StateMachine;

public class PlayerState : BaseState {
  protected IMatchController Controller { get; private set; }
  public PlayerState(IMatchController controller) {
    Controller = controller;
  }
}
