namespace HOB;

using System.Collections.Generic;
using GameplayFramework;
using HOB.GameEntity;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  public int CurrentPlayerIndex { get; set; }
  public int CurrentRound { get; set; }

  public GameBoard GameBoard { get; set; }
  public List<Entity> Entities { get; set; }
}
