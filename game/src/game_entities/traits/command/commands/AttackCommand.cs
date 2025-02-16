namespace HOB.GameEntity;

using System.Linq;
using Godot;

[GlobalClass]
public partial class AttackCommand : Command {
  [Export] public AttackTrait EntityAttackTrait { get; private set; }

  public override void _Ready() {
    base._Ready();

    EntityAttackTrait.AttackFinished += Finish;
  }

  public bool TryAttack(Entity entity) {
    if (IsAvailable() && EntityAttackTrait.GetAttackableEntities().entities.Contains(entity)) {
      Use();
      EntityAttackTrait.Attack(entity);
      return true;
    }
    return false;
  }

  public (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities() {
    return EntityAttackTrait.GetAttackableEntities();
  }
}
