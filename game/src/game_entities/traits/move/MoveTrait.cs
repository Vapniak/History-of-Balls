namespace HOB.GameEntity;

using System.Threading.Tasks;
using Godot;

[GlobalClass]
public abstract partial class MoveTrait : Trait {
  [Signal] public delegate void MoveFinishedEventHandler();

  public override void _Ready() {
    base._Ready();

    Entity.CellChanged += () => {
      foreach (var entity in Entity.EntityManagment.GetEntitiesOnCell(Entity.Cell)) {
        if (entity.TryGetTrait<ClaimableTrait>(out var claimableTrait)) {
          if (Entity.TryGetOwner(out var owner)) {
            claimableTrait.ClaimBy(owner);
          }
        }
      }
    };
  }
  public virtual GameCell[] GetReachableCells() {
    return Entity.Cell.ExpandSearch(GetStat<MovementStats>().MovePoints, IsCellReachable);
  }

  public virtual GameCell[] FindPath(GameCell cell) {
    return Entity.Cell.FindPathTo(cell, GetStat<MovementStats>().MovePoints, IsCellReachable);
  }

  public virtual async Task Move(GameCell targetCell) {
    EmitSignal(SignalName.MoveFinished);
  }

  public abstract bool IsCellReachable(GameCell from, GameCell to);
}
