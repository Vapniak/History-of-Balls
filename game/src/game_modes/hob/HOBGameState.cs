namespace HOB;

using System;
using GameplayFramework;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class HOBGameState : GameState, IPlayerManagmentGameState, IMatchGameState {
  public Array<PlayerState> PlayerArray { get; set; }

  public int CurrentPlayerIndex { get; set; }
  public int CurrentRound { get; set; }
  public GameBoard GameBoard { get; set; }
}
