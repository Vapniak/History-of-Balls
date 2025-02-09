namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public partial class AttackCommand : Command {
  [Export] public AttackTrait EntityAttackTrait { get; private set; }

  public override void _Ready() {
    base._Ready();

    EntityAttackTrait.AttackFinished += Finish;
  }

  public bool TryAttack(Entity entity) {
    if (EntityAttackTrait.TryAttack(entity)) {
      Use();
      // for now
      Finish();
      return true;
    }
    return false;
  }
}
