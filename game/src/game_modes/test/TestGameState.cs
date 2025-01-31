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


  public override void Init() {
    base.Init();

    TurnChangedEvent?.Invoke(0, 0);
  }
  // TODO: better turn managment
  public void NextTurn() {
    if (CurrentPlayerIndex >= PlayerArray.Count) {
      CurrentRound++;
      CurrentPlayerIndex = 0;

    }
    else {
      CurrentPlayerIndex++;
    }

    TurnChangedEvent?.Invoke(CurrentPlayerIndex, CurrentRound);
  }

  public bool IsCurrentTurn(IMatchController controller) => controller.GetPlayerState().PlayerIndex == CurrentPlayerIndex;
}
