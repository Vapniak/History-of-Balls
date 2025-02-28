namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class GenerateIncomeCommand : Command {
  [Export] public IncomeTrait EntityIncomeTrait { get; private set; }

  public override void OnTurnEnded() {
    base.OnTurnEnded();

    if (GetEntity().TryGetOwner(out var owner)) {
      if (CanBeUsed(owner)) {
        Use();
        EntityIncomeTrait.GenerateIncome();
        Finish();
      }
    }
  }
}
