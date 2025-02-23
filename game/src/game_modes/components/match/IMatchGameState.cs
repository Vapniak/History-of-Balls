namespace HOB;

using System;
using GameplayFramework;
using Godot;

public interface IMatchGameState : IGameState, IPlayerManagmentGameState {
  public event Action TurnChangedEvent;
  public event Action TurnStartedEvent;
  public event Action TurnEndedEvent;

  // TODO: add match state like: before start, in progress, end

  public event Action RoundStartedEvent;

  public GameBoard GameBoard { get; }
  public int CurrentPlayerIndex { get; }
  public int CurrentRound { get; }

  public void NextTurn();
  public void Pause();
  public bool IsCurrentTurn(IMatchController controller);
}
