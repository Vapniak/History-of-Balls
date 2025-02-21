namespace HOB;

using System;
using GameplayFramework;
using Godot;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  public delegate void TurnChangedEventHandler(int playerIndex);
  public delegate void RoundChangedEventHandler(int roundNumber);
  public event TurnChangedEventHandler TurnChangedEvent;

  // TODO: add match state like: before start, in progress, end

  public event RoundChangedEventHandler RoundStartedEvent;

  public GameBoard GameBoard { get; }
  public int CurrentPlayerIndex { get; }
  public int CurrentRound { get; }

  public void NextTurn();
  public bool IsCurrentTurn(IMatchController controller);
}
