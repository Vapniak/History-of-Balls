namespace HOB.GameEntity;

using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public abstract partial class MovementType : Resource {
  [Signal] public delegate void MoveFinishedEventHandler();

  public MoveTrait MoveTrait { get; set; }
  public abstract bool IsCellReachable(GameCell from, GameCell to);
  public virtual async Task StartMoveOn(GameCell[] path) { }
}
