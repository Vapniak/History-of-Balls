namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;
using System;

[GlobalClass]
public partial class AIController : Controller, IMatchController {
  // TODO: add behavior tree
  public Action<CubeCoord> CoordsClicked { get; set; }

  public bool IsOwnTurn() => GetGameState().CurrentPlayerIndex == GetPlayerState().PlayerIndex;
  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;
}
