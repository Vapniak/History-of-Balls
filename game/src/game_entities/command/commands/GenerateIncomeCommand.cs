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
  public override void OnTurnStarted() {
    base.OnTurnStarted();

    if (GetEntity().TryGetOwner(out var owner) && owner.IsCurrentTurn()) {
      Use();
    }
  }
}
