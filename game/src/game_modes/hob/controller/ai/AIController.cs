namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

[GlobalClass]
public partial class AIController : Controller, IMatchController {
  public event Action EndTurnEvent;

  public Country Country { get; set; }

  private IEntityManagment EntityManagment => GetGameMode().GetEntityManagment();

  // TODO: implement Monte Carlo Tree Search algorithm for ai

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  private void EndTurn() {
    EndTurnEvent?.Invoke();
  }

  public async Task OnOwnTurnStarted() {
    // FIXME: temp fix for ai ending turn before checking for win condition in game mode
    await Task.Delay(1000);
    EndTurn();
  }

  public void OwnTurnStarted() {
    OnOwnTurnStarted();
  }

  public void OwnTurnEnded() { }

  public void OnGameStarted() { }

  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;
}
