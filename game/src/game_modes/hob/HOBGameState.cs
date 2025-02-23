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

  public event Action TurnStartedEvent;
  public event Action TurnChangedEvent;
  public event Action RoundStartedEvent;
  public event Action TurnEndedEvent;

  public void NextTurn() {
    TurnEndedEvent?.Invoke();

    CurrentPlayerIndex++;

    if (CurrentPlayerIndex >= PlayerArray.Count) {
      CurrentRound++;
      CurrentPlayerIndex = 0;
      RoundStartedEvent?.Invoke();
    }

    TurnChangedEvent?.Invoke();
    TurnStartedEvent?.Invoke();
  }

  public bool IsCurrentTurn(IMatchController controller) => controller.GetPlayerState().PlayerIndex == CurrentPlayerIndex;
}
