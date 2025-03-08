namespace HOB.GameEntity.Test;

using System.Linq;
using HOB.GameEntity;

[RequiresTrait<MoveTrait>]
public class TestMoveCommand : GameCommand<MoveParameters> {
  public override bool CanExecute(Entity entity, MoveParameters parameters) {
    return true;
  }
  public override void Execute(Entity entity, MoveParameters parameters) {
    if (entity.TryGetTrait<MoveTrait>(out var moveTrait)) {
      moveTrait.Move(parameters.Path);
    }
  }
}
