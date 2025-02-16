namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public abstract partial class MovementType : Resource {
  [Signal] public delegate void MoveFinishedEventHandler();

  public MoveTrait MoveTrait { get; set; }
  public abstract bool IsCellReachable(GameCell from, GameCell to);
  public abstract void StartMoveOn(GameCell[] path);
}
