namespace HOB;

using System;
using System.Collections.Generic;
using GameplayFramework;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass]
public partial class HOBGameState : GameState, IMatchGameState {
  public Array<PlayerState> PlayerArray { get; set; }

  public int CurrentPlayerIndex { get; set; }
  public int CurrentRound { get; set; }
  public GameBoard GameBoard { get; set; }
  public List<Entity> Entities { get; set; }
}
