namespace HOB;

using System;
using GameplayFramework;
using Godot;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  public event Action<int> NextTurnEvent;
  public event Action<int> NextRoundEvent;

  public GameBoard GameBoard { get; }
  public int CurrentPlayerIndex { get; }
  public int CurrentRound { get; }

  public void NextTurn();
}
