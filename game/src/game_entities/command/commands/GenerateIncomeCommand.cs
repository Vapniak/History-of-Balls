namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class GenerateIncomeCommand : Command {
  [Export] public IncomeTrait EntityIncomeTrait { get; private set; }

  protected override void Use() {
    base.Use();
    EntityIncomeTrait.GenerateIncome();
    Finish();
  }
  public override void OnTurnEnded() {
    base.OnTurnEnded();

    if (GetEntity().TryGetOwner(out var owner)) {
      Use();
    }
  }
}
