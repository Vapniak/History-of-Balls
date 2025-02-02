namespace HOB;

using System;
using GameplayFramework;
using Godot;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  public delegate void TurnChangedEventHandler(int playerIndex);
  public delegate void RoundChangedEventHandler(int roundNumber);
  public event TurnChangedEventHandler TurnChangedEvent;

  public event RoundChangedEventHandler RoundStartedEvent;
  public event RoundChangedEventHandler RoundEndedEvent;

  public GameBoard GameBoard { get; }
  public int CurrentPlayerIndex { get; }
  public int CurrentRound { get; }

  public void NextTurn();
  public bool IsCurrentTurn(IMatchController controller);
}
