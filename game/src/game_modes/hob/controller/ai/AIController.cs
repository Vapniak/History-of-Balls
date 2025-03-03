namespace HOB;

using GameplayFramework;
using Godot;
using HOB.GameEntity;
using System;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class AIController : Controller, IMatchController {
  public event Action EndTurnEvent;

  private GameBoard GameBoard { get; set; }

  public Country Country { get; set; }

  // TODO: implement Monte Carlo Tree Search algorithm for ai

  public override void _Ready() {
    base._Ready();

    GameBoard = GetGameState().GameBoard;
  }

  public bool IsCurrentTurn() => GetGameState().IsCurrentTurn(this);
  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  private void EndTurn() {
    EndTurnEvent?.Invoke();
  }

  public async Task OnOwnTurnStarted() {
    foreach (var entity in GameBoard.GetOwnedEntities(this)) {
      await Decide(entity);
    }

    // FIXME: temp fix for ai ending turn before checking for win condition in game mode
    await Task.Delay(1000);
    EndTurn();
  }

  public void OwnTurnStarted() {
    OnOwnTurnStarted();
  }

  public void OwnTurnEnded() { }

  public void OnGameStarted() { }

  private async Task Decide(Entity entity) {
    if (entity.TryGetTrait<CommandTrait>(out var commandTrait)) {
      while (true) {
        Command bestCommand = null;
        object bestParameters = null;
        var bestUtility = 0f;

        foreach (var command in commandTrait.GetCommands()) {
          if (command.CanBeUsed(this)) {
            if (command is MoveCommand moveCommand) {
              var utility = CalculateMoveUtility(moveCommand, out var closestEnemyCell);
              if (utility > bestUtility) {
                bestUtility = utility;
                bestCommand = moveCommand;
                bestParameters = closestEnemyCell;
              }
            }
            else if (command is AttackCommand attackCommand) {
              var (attackableEntities, _) = attackCommand.GetAttackableEntities();
              foreach (var target in attackableEntities) {
                var utility = CalculateAttackUtility(attackCommand, target);
                if (utility > bestUtility) {
                  bestUtility = utility;
                  bestCommand = attackCommand;
                  bestParameters = target;
                }
              }
            }
          }
        }

        if (bestCommand != null) {
          if (bestCommand is AttackCommand attack && bestParameters is Entity target) {
            if (attack.TryAttack(this, target)) {
              await ToSignal(attack.AttackTrait, AttackTrait.SignalName.AttackFinished);
            }
          }
          else if (bestCommand is MoveCommand move && bestParameters is GameCell cell) {
            if (move.TryMove(this, cell)) {
              await ToSignal(move.MoveTrait, MoveTrait.SignalName.MoveFinished);
            }
          }
        }
        else {
          break;
        }
      }
    }
  }


  private Command[] GenerateAvailableCommands(CommandTrait commandTrait) {
    return commandTrait.GetCommands().Where(c => c.CanBeUsed(this)).ToArray();
  }

  private float CalculateMoveUtility(MoveCommand moveCommand, out GameCell closestEnemyCell) {
    float utility = 0;

    // TODO: try claim
    var closestEnemy = FindClosestEnemy(moveCommand.GetEntity());
    closestEnemyCell = null;
    if (closestEnemy != null) {
      closestEnemyCell = closestEnemy.Cell;
      return 1;
    }

    return utility;
  }

  private Entity FindClosestEnemy(Entity currentEntity) {
    Entity closestEnemy = null;
    var closestDistance = float.MaxValue;

    foreach (var enemy in GameBoard.GetEnemyEntities(this).Union(GameBoard.GetNotOwnedEntities())) {
      float distance = currentEntity.Cell.Coord.Distance(enemy.Cell.Coord);
      if (distance < closestDistance) {
        closestDistance = distance;
        closestEnemy = enemy;
      }
    }

    return closestEnemy;
  }

  private float CalculateAttackUtility(AttackCommand attackCommand, Entity target) {
    float utility = 0;

    if (!attackCommand.AttackTrait.CanBeAttacked(target)) {
      return -1;
    }

    attackCommand.GetEntity().TryGetStat<AttackStats>(out var attackStat);
    if (target.TryGetStat<HealthStats>(out var healthStats)) {
      utility += 100 - healthStats.CurrentHealth;

      if (healthStats.CurrentHealth < attackStat.Damage) {
        utility -= 20;
      }
    }

    float distance = attackCommand.GetEntity().Cell.Coord.Distance(target.Cell.Coord);
    utility -= distance * 5;

    return utility;
  }

  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
}
