namespace HOB.GameEntity;

using Godot;
using System;

[GlobalClass]
public partial class GenerateIncomeCommand : Command {
  [Export] public IncomeTrait EntityIncomeTrait { get; private set; }

  protected override void Use() {
    base.Use();
    EntityIncomeTrait.GenerateIncome();
  }
}
