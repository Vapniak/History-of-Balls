namespace HOB;

using GameplayFramework;
using Godot;
using HexGridMap;
using HOB.GameEntity;
using System;
using System.Linq;
using System.Threading.Tasks;

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

  public async Task OwnTurnStarted() {
    foreach (var entity in GameBoard.GetOwnedEntities(this)) {
      await Decide(entity);
    }

    EndTurn();
  }
  public void OwnTurnEnded() { }

  public void OnGameStarted() { }

  private async Task Decide(Entity entity) {
    if (entity.TryGetTrait<CommandTrait>(out var commandTrait)) {
      if (TryAttack(commandTrait)) {
        await ToSignal(commandTrait, CommandTrait.SignalName.CommandFinished);
      }
      else {
        if (TryMove(commandTrait)) {
          await ToSignal(commandTrait, CommandTrait.SignalName.CommandFinished);
        }

        if (TryAttack(commandTrait)) {
          await ToSignal(commandTrait, CommandTrait.SignalName.CommandFinished);
        }
      }
    }
  }

  private bool TryMove(CommandTrait commandTrait) {
    if (commandTrait.TryGetCommand<MoveCommand>(out var moveCommand)) {
      GameCell closestEnemyCell = null;
      foreach (var enemy in GameBoard.GetEnemyEntities(this)) {
        if (!enemy.TryGetTrait<HealthTrait>(out _)) {
          continue;
        }

        closestEnemyCell ??= enemy.Cell;

        if (commandTrait.Entity.Cell.Coord.Distance(enemy.Cell.Coord) < commandTrait.Entity.Cell.Coord.Distance(closestEnemyCell.Coord)) {
          closestEnemyCell = enemy.Cell;
        }
      }
      return moveCommand.TryMove(closestEnemyCell);
    }

    return false;
  }

  private bool TryAttack(CommandTrait commandTrait) {
    if (commandTrait.TryGetCommand<AttackCommand>(out var attackCommand)) {
      foreach (var enemy in attackCommand.GetAttackableEntities().entities) {
        // TODO: attack closest enemy
        return attackCommand.TryAttack(enemy);
      }
    }

    return false;
  }

  void IMatchController.OwnTurnStarted() {
    OwnTurnStarted();
  }

  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
}
