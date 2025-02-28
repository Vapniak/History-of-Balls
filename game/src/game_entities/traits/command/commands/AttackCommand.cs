namespace HOB.GameEntity;

using System.Linq;
using Godot;

[GlobalClass]
public partial class AttackCommand : Command {
  [Export] public AttackTrait AttackTrait { get; private set; }

  public override void _Ready() {
    base._Ready();

    AttackTrait.AttackFinished += Finish;
  }

  public bool TryAttack(IMatchController caller, Entity entity) {
    if (CanBeUsed(caller)) {
      Use();
      AttackTrait.Attack(entity);
      return true;
    }

    return false;
  }

  public (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities() {
    return AttackTrait.GetAttackableEntities();
  }
}
