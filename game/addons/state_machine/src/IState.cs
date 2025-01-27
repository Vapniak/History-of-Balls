namespace StateMachine;


public interface IState {
  public void Enter();
  public void Update(double delta);
  public void Exit();
}
