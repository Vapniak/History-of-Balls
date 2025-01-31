namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;
using System;

[GlobalClass]
public partial class AIController : Controller, IMatchController {
  public event Action EndTurnEvent;

  // TODO: add behavior tree
  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;
  public bool IsCurrentTurn() => GetGameState().IsCurrentTurn(this);
}
