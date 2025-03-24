namespace HOB;

using System.Diagnostics.Contracts;
using GameplayFramework;
using HOB.GameEntity;

public interface IMatchPlayerState : IPlayerState {
  public Country? Country { get; set; }
  public int PrimaryResourceValue { get; set; }
  public int SecondaryResourceValue { get; set; }
}
