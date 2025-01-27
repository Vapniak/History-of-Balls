namespace StateMachine;


public class BaseState : IState {
  public virtual void Enter() { }
  public virtual void Exit() { }
  public virtual void Update(double delta) { }
}
