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

  public event Action<int> NextTurnEvent;

  public event Action<int> NextRoundEvent;

  public void NextTurn() {
    if (CurrentPlayerIndex >= PlayerArray.Count) {
      CurrentRound++;
      CurrentPlayerIndex = 0;

      NextRoundEvent(CurrentRound);
    }
    else {
      CurrentPlayerIndex++;
    }

    NextTurnEvent(CurrentPlayerIndex);
  }
}
