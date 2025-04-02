namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using Godot;
using HOB.GameEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class AIController : Controller, IMatchController {
  public event Action? EndTurnEvent;
  public Country? Country { get; set; }

  private IEntityManagment EntityManagment => GetGameMode().GetEntityManagment();
  private HOBGameMode GameMode => GetGameMode();

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public async Task StartDecisionMaking() {
    await Task.Delay(1000); // Temporary win condition check buffer

    foreach (var entity in EntityManagment.GetOwnedEntites(this)) {
      await ProcessEntityActions(entity);
    }

    await Task.Delay(1000);

    EndTurn();
  }

  private async Task ProcessEntityActions(Entity entity) {
    while (true) {
      var (bestAbility, bestEventData) = await FindBestAction(entity);
      if (bestAbility == null) {
        break;
      }

      var result = await entity.AbilitySystem.TryActivateAbilityAsync(bestAbility, bestEventData);
      if (!result) {
        break;
      }
    }
  }

  private async Task<(GameplayAbilityInstance?, GameplayEventData?)> FindBestAction(Entity entity) {
    GameplayAbilityInstance? bestAbility = null;
    GameplayEventData? bestEventData = null;
    var bestScore = 0f;

    foreach (var ability in entity.AbilitySystem.GetGrantedAbilities()) {
      if (!ability.CanActivateAbility(new() { Activator = this })) {
        continue;
      }

      var (score, eventData) = await EvaluateAbility(entity, ability);
      if (score > bestScore) {
        bestScore = score;
        bestAbility = ability;
        bestEventData = eventData;
      }
    }

    return (bestAbility, bestEventData);
  }

  private async Task<(float score, GameplayEventData? data)> EvaluateAbility(Entity entity, GameplayAbilityInstance ability) {
    return ability switch {
      MoveAbilityResource.Instance moveAbility => await EvaluateMoveAbility(entity, moveAbility),
      AttackAbilityResource.Instance attackAbility => await EvaluateAttackAbility(entity, attackAbility),
      //EntityProductionAbilityResource.Instance produceAbility => await EvaluateProduceAbility(entity, produceAbility),
      _ => (0, null)
    };
  }

  private async Task<(float Score, GameplayEventData? Data)> EvaluateMoveAbility(Entity entity, MoveAbilityResource.Instance ability) {
    var bestScore = float.MinValue;
    GameCell? bestCell = null;

    foreach (var enemy in GetPotentialEnemies(entity)) {
      var path = ability.FindPathTo(enemy.Cell);
      if (path == null || path.Length == 0) {
        continue;
      }

      var score = CalculateMoveScore(entity, enemy, path);
      if (score > bestScore) {
        bestScore = score;
        bestCell = path.Last();
      }
    }

    return bestCell != null
        ? (bestScore, new GameplayEventData() { Activator = this, TargetData = new MoveTargetData() { Cell = bestCell } })
        : (0, null);
  }

  private async Task<(float Score, GameplayEventData? Data)> EvaluateAttackAbility(Entity entity, AttackAbilityResource.Instance ability) {
    var bestScore = float.MinValue;
    Entity? bestTarget = null;

    var (entities, _) = ability.GetAttackableEntities();
    foreach (var target in entities) {
      var score = CalculateAttackScore(entity, ability, target);
      if (score > bestScore) {
        bestScore = score;
        bestTarget = target;
      }
    }

    return bestTarget != null
        ? (bestScore, new GameplayEventData() {
          Activator = this,
          TargetData = new AttackTargetData() { TargetAbilitySystem = bestTarget.AbilitySystem }
        })
        : (0, null);
  }

  // private async Task<(float Score, GameplayEventData? Data)> EvaluateProduceAbility(Entity entity, EntityProductionAbilityResource.Instance ability) {
  //   var bestScore = float.MinValue;
  //   ProducedEntityData? bestProduction = null;

  //   foreach (var production in ability.GetProducibleEntities()) {
  //     var score = CalculateProductionScore(entity, production);
  //     if (score > bestScore) {
  //       bestScore = score;
  //       bestProduction = production;
  //     }
  //   }

  //   return bestProduction != null
  //       ? (bestScore, CreateProductionEvent(bestProduction))
  //       : (0, null);
  // }

  private float CalculateMoveScore(Entity entity, Entity target, IEnumerable<GameCell> path) {
    var distanceScore = 1f / entity.Cell.Coord.Distance(target.Cell.Coord);
    // var attackScore = CanAttackFrom(entity, path.Last(), target) ? 2f : 0f;
    // var threatScore = CalculateThreatAvoidance(entity, path.Last());

    return distanceScore * 0.6f; //+ attackScore * 0.3f + threatScore * 0.1f;
  }

  private float CalculateAttackScore(Entity attacker, AttackAbilityResource.Instance ability, Entity target) {
    // var damage = ability.CalculatePotentialDamage(attacker, target);
    // var killBonus = damage >= target.Health ? 3f : 0f;
    // var valueScore = target.GetStrategicValue() * 0.5f;

    return 1f;//(damage / target.Health) * 2f + killBonus + valueScore;
  }

  // private float CalculateProductionScore(Entity producer, ProducedEntityData production) {
  //   var forces = EntityManagment.GetOwnedEntites(this);
  //   var unitCount = forces.Count(e => e.EntityType == production.EntityType);
  //   var typeScore = production.EntityType switch {
  //     EntityType.Ranged when unitCount < 3 => 2f,
  //     EntityType.Melee when unitCount < 5 => 1.5f,
  //     _ => 0.5f
  //   };

  //   return typeScore * production.StrategicWeight;
  // }

  private IEnumerable<Entity> GetPotentialEnemies(Entity entity) =>
      EntityManagment.GetEnemyEntities(this)
          .Union(EntityManagment.GetNotOwnedEntities());

  // private IEnumerable<Entity> GetAttackTargets(Entity entity, AttackAbilityResource.Instance ability) =>
  //     GetPotentialEnemies(entity)
  //         .Where(e => ability.CanTarget(entity, e));

  // private bool CanAttackFrom(Entity entity, GameCell position, Entity target) =>
  //     position.DistanceTo(target.Cell) <= entity.GetStat<AttackStats>().Range;

  public void OwnTurnStarted() => _ = StartDecisionMaking();
  public void OwnTurnEnded() { }
  public void OnGameStarted() { }
  private void EndTurn() => EndTurnEvent?.Invoke();
  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;
}