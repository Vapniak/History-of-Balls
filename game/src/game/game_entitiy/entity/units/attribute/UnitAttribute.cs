namespace HOB;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public abstract partial class UnitAttribute : Node3D {
  public abstract Task DoAction(Vector3 toPosition);
  public abstract Task Reset();
}