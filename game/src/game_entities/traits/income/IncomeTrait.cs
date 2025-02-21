namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public partial class IncomeTrait : Trait {
  public void GenerateIncome() {
    var stats = GetStat<IncomeStats>();
    switch (stats.IncomeType) {
      case IncomeType.Primary:
        Entity.OwnerController.GetPlayerState().PrimaryResourceType.Value += stats.Value;
        break;
      case IncomeType.Secondary:
        Entity.OwnerController.GetPlayerState().SecondaryResourceType.Value += stats.Value;
        break;
      default:
        GD.PrintErr("Invalid income type!");
        break;
    }
  }
}
