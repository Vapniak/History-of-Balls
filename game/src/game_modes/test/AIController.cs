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

    GetGameState().TurnChangedEvent += OnTurnChanged;

    GameBoard = GetGameState().GameBoard;
  }

  public bool IsCurrentTurn() => GetGameState().IsCurrentTurn(this);
  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  private void OnTurnChanged(int playerIndex) {
    if (IsCurrentTurn()) {
      var timer = new Timer();
      AddChild(timer);
      timer.Timeout += () => {
        EndTurnEvent?.Invoke();
        timer.QueueFree();
      };
      timer.Start(1);
    }
  }
}
