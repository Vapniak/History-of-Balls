namespace HOB;

using System;
using GameplayFramework;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TestGameState : GameState, IPlayerManagmentGameState, IPauseGameState, IMatchGameState {
  public GameBoard GameBoard { get; set; }

  public Array<PlayerState> PlayerArray { get; set; }
  public bool PauseGame { get; private set; } = true;

  public int CurrentPlayerIndex { get; private set; }
  public int CurrentRound { get; private set; }

  public event IMatchGameState.TurnChangedEventHandler TurnChangedEvent;
  public event IMatchGameState.RoundChangedEventHandler RoundStartedEvent;
  public event IMatchGameState.RoundChangedEventHandler RoundEndedEvent;

  // TODO: better turn managment
  public void NextTurn() {
    if (CurrentPlayerIndex >= PlayerArray.Count - 1) {
      CurrentRound++;
      CurrentPlayerIndex = 0;
      RoundEndedEvent?.Invoke(CurrentRound - 1);
      RoundStartedEvent?.Invoke(CurrentRound);
    }
    else {
      CurrentPlayerIndex++;
    }

    TurnChangedEvent?.Invoke(CurrentPlayerIndex);
  }

  public bool IsCurrentTurn(IMatchController controller) => controller.GetPlayerState().PlayerIndex == CurrentPlayerIndex;
}
