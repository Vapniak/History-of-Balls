namespace HOB;

using System;
using GameplayFramework;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class HOBGameState : GameState, IPlayerManagmentGameState, IPauseGameState, IMatchGameState {
  public GameBoard GameBoard { get; set; }

  public Array<PlayerState> PlayerArray { get; set; }
  public bool PauseGame { get; private set; } = true;

  public int CurrentPlayerIndex { get; private set; }
  public int CurrentRound { get; private set; }

  public event IMatchGameState.TurnChangedEventHandler TurnStartedEvent;
  public event IMatchGameState.TurnChangedEventHandler TurnChangedEvent;
  public event IMatchGameState.RoundChangedEventHandler RoundStartedEvent;

  // TODO: better turn managment
  public void NextTurn() {
    CurrentPlayerIndex++;

    if (CurrentPlayerIndex >= PlayerArray.Count) {
      CurrentRound++;
      CurrentPlayerIndex = 0;
      RoundStartedEvent?.Invoke(CurrentRound);
    }

    TurnChangedEvent?.Invoke(CurrentPlayerIndex);
    TurnStartedEvent?.Invoke(CurrentPlayerIndex);
  }

  public bool IsCurrentTurn(IMatchController controller) => controller.GetPlayerState().PlayerIndex == CurrentPlayerIndex;
}
