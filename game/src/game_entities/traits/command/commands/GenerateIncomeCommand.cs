namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class GenerateIncomeCommand : Command {
  [Export] public IncomeTrait EntityIncomeTrait { get; private set; }

  public override void OnTurnEnded() {
    base.OnTurnEnded();

    if (IsAvailable()) {
      Use();
      EntityIncomeTrait.GenerateIncome();
      Finish();
    }
  }
}
