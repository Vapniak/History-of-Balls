namespace StateMachine;

using Godot;
using System;

[GlobalClass]
public partial class StateMachine : Node {
  public IState CurrentState;

  public void ChangeStateTo(IState newState) {
    if (newState == null) {
      return;
    }

    CurrentState?.Exit();
    CurrentState = newState;
    CurrentState.Enter();
  }

  public override void _Process(double delta) {
    CurrentState?.Update(delta);
  }
}
