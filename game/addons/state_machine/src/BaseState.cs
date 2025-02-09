namespace StateMachine;

using Godot;

[GlobalClass]
public partial class BaseState : Node, IState {
  public virtual void Enter() { }
  public virtual void Exit() { }
  public virtual void Update(double delta) { }
}
