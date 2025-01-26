namespace HOB;

using System;
using GameplayFramework;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  public GameBoard GameBoard { get; }
}
