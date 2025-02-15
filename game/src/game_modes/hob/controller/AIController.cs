namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;
using System;

[GlobalClass]
public partial class AIController : Controller, IMatchController {
  public event Action EndTurnEvent;

  private GameBoard GameBoard { get; set; }

  // TODO: add behavior tree

  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;
  }

  public bool IsCurrentTurn() => GetGameState().IsCurrentTurn(this);
  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  private void EndTurn() {
    EndTurnEvent?.Invoke();
  }

  public void OwnTurnStarted() {
    var timer = new Timer();
    AddChild(timer);
    timer.Timeout += () => {
      EndTurn();
      timer.QueueFree();
    };
    timer.Start(1);
  }
  public void OwnTurnEnded() { }

  public void OnGameStarted() { }

  private void Decide(Entity entity) {

  }
}
