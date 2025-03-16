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
    foreach (var entity in EntityManagment.GetOwnedEntites(this)) {
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
          if (command.CanBeUsed()) {
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
            else if (command is ProduceEntityCommand produceEntityCommand) {
              if (produceEntityCommand.GetEntity().TryGetStat<EntityProducerStats>(out var producerStats)) {
                var producable = producerStats.ProducedEntities;
                foreach (var p in producable) {
                  var utility = CalculateProduceUtility(produceEntityCommand, p);
                  if (utility > bestUtility) {
                    bestUtility = utility;
                    bestCommand = produceEntityCommand;
                    bestParameters = p;
                  }
                }
              }
            }
          }
        }

        if (bestCommand != null) {
          if (bestCommand is AttackCommand attack && bestParameters is Entity target) {
            if (attack.TryAttack(target)) {
              await ToSignal(attack.AttackTrait, AttackTrait.SignalName.AttackFinished);
            }
          }
          else if (bestCommand is MoveCommand move && bestParameters is GameCell cell) {
            if (move.TryMove(cell)) {
              await ToSignal(move.MoveTrait, MoveTrait.SignalName.MoveFinished);
            }
          }
          else if (bestCommand is ProduceEntityCommand produceEntity && bestParameters is ProducedEntityData data) {
            if (produceEntity.TryStartProduceEntity(data)) {

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
    return commandTrait.GetCommands().Where(c => c.CanBeUsed()).ToArray();
  }

  private float CalculateMoveUtility(MoveCommand moveCommand, out GameCell bestCell) {
    float utility = 0;

    var closestEnemy = FindClosestEnemy(moveCommand.GetEntity());
    bestCell = null;
    if (closestEnemy != null) {
      bestCell ??= closestEnemy.Cell;
      if (closestEnemy.TryGetTrait<HealthTrait>(out _) && moveCommand.GetEntity().TryGetStat<AttackStats>(out var stats)) {
        foreach (var cell in moveCommand.GetReachableCells()) {
          var distance = cell.Coord.Distance(moveCommand.GetEntity().Cell.Coord);
          if (distance >= bestCell.Coord.Distance(moveCommand.GetEntity().Cell.Coord) && cell.Coord.Distance(closestEnemy.Cell.Coord) <= stats.Range) {
            bestCell = cell;
          }
        }
      }
    }

    if (bestCell == null) {
      return 0;
    }

    return 1;
  }

  private Entity FindClosestEnemy(Entity currentEntity) {
    Entity closestEnemy = null;
    var closestDistance = float.MaxValue;

    foreach (var enemy in EntityManagment.GetEnemyEntities(this).Union(EntityManagment.GetNotOwnedEntities())) {
      float distance = currentEntity.Cell.Coord.Distance(enemy.Cell.Coord);
      if (distance < closestDistance) {
        closestDistance = distance;
        closestEnemy = enemy;
      }
    }

    return closestEnemy;
  }

  private float CalculateAttackUtility(AttackCommand attackCommand, Entity target) {
    if (!attackCommand.AttackTrait.CanBeAttacked(target)) {
      return -1;
    }

    var utility = 0f;

    var attackStats = attackCommand.GetEntity().GetStat<AttackStats>();

    // will kill
    if (attackStats.Damage >= target.GetStat<HealthStats>().Health) {
      utility += 1;
    }
    else {
      utility += 0.5f;
    }

    return utility;
  }

  private float CalculateProduceUtility(ProduceEntityCommand produceEntityCommand, ProducedEntityData data) {
    float utility = 0;

    if (!produceEntityCommand.CanEntityBeProduced(data)) {
      return 0;
    }

    var entities = EntityManagment.GetOwnedEntites(this);

    var rangedCount = entities.Count(e => e.TryGetStat<AttackStats>(out var stats) && stats.Range > 1);
    var meleeCount = entities.Count(e => e.TryGetStat<AttackStats>(out var stats) && stats.Range == 1);

    if (data.Entity.Stats.TryGetStat<AttackStats>(out var stat) && stat.Range > 1 && meleeCount < rangedCount) {
      return 0;
    }
    else {
      utility = 1;
    }

    return utility;
  }

  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;
}
