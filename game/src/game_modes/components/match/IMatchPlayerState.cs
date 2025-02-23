namespace HOB;

using GameplayFramework;
using HOB.GameEntity;

public interface IMatchPlayerState : IPlayerState {
  public ResourceType PrimaryResourceType { get; set; }
  public ResourceType SecondaryResourceType { get; set; }

  public ResourceType GetResourceType(IncomeType incomeType) {
    return incomeType switch {
      IncomeType.Primary => PrimaryResourceType,
      IncomeType.Secondary => SecondaryResourceType,
      _ => null,
    };
  }
}
