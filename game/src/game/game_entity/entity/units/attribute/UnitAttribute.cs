namespace HOB;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public abstract partial class UnitAttribute : Node3D {
  [Signal] public delegate void ActedEventHandler();
  [Signal] public delegate void ResetedEventHandler();
  public virtual Task DoAction(Vector3 toPosition) {
    EmitSignal(SignalName.Acted);
    return Task.CompletedTask;
  }
  public virtual Task Reset() {
    EmitSignal(SignalName.Reseted);
    return Task.CompletedTask;
  }
}