namespace HOB.GameEntity;

using Godot;

[GlobalClass]
public partial class IncomeTrait : Trait {
  public void GenerateIncome() {
    if (Entity.TryGetOwner(out var owner)) {
      var stats = GetStat<IncomeStats>();
      switch (stats.IncomeType) {
        case IncomeType.Primary:
          owner.GetPlayerState().PrimaryResourceType.Value += stats.Value;
          break;
        case IncomeType.Secondary:
          owner.GetPlayerState().SecondaryResourceType.Value += stats.Value;
          break;
        default:
          GD.PrintErr("Invalid income type!");
          break;
      }
    }
  }
}
