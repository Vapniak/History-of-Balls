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
    await Task.Delay(500); // Temporary win condition check buffer

    await ProcessEntityActions();

    await Task.Delay(500);

    EndTurn();
  }

  private async Task ProcessEntityActions() {
    while (true) {
      (Entity? bestEntity, GameplayAbilityInstance? bestAbility, GameplayEventData? bestData, var bestScore) =
          (null, null, null, 0f);

      foreach (var entity in EntityManagment.GetOwnedEntites(this)) {
        var (ability, data, score) = FindBestAction(entity);
        if (ability != null && score > bestScore) {
          bestEntity = entity;
          bestAbility = ability;
          bestData = data;
          bestScore = score;
        }
      }

      if (bestEntity != null && bestAbility != null) {
        GD.PrintS($"AI attempting: {bestEntity.EntityName} -> {bestAbility.AbilityResource.AbilityName}");

        if (bestAbility is MoveAbilityResource.Instance or AttackAbilityResource.Instance) {
          var success = await bestEntity.AbilitySystem.TryActivateAbilityAsync(bestAbility, bestData);

          if (!success) {
            GD.PrintS("Activation failed! Re-evaluating...");
            await Task.Delay(100);
          }
        }
        else {
          bestEntity.AbilitySystem.TryActivateAbility(bestAbility, bestData);
        }
      }
      else {
        break;
      }
    }
  }

  private (GameplayAbilityInstance?, GameplayEventData?, float score) FindBestAction(Entity entity) {
    GameplayAbilityInstance? bestAbility = null;
    GameplayEventData? bestEventData = null;
    var bestScore = 0f;

    foreach (var ability in entity.AbilitySystem.GetGrantedAbilities()) {
      if (!ability.CanActivateAbility(new() { Activator = this })) {
        continue;
      }

      var (score, eventData) = EvaluateAbility(entity, ability);
      if (score > bestScore) {
        bestScore = score;
        bestAbility = ability;
        bestEventData = eventData;
      }
    }

    return (bestAbility, bestEventData, bestScore);
  }

  private (float score, GameplayEventData? data) EvaluateAbility(Entity entity, GameplayAbilityInstance ability) {
    return ability switch {
      MoveAbilityResource.Instance moveAbility => EvaluateMoveAbility(entity, moveAbility),
      AttackAbilityResource.Instance attackAbility => EvaluateAttackAbility(entity, attackAbility),
      EntityProductionAbilityResource.Instance produceAbility => EvaluateProduceAbility(entity, produceAbility),
      _ => (0, null)
    };
  }

  private (float Score, GameplayEventData? Data) EvaluateMoveAbility(Entity entity, MoveAbilityResource.Instance ability) {
    var bestScore = 0f;
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

  private (float Score, GameplayEventData? Data) EvaluateAttackAbility(Entity entity, AttackAbilityResource.Instance ability) {
    var bestScore = 0f;
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

  private (float Score, GameplayEventData? Data) EvaluateProduceAbility(Entity entity, EntityProductionAbilityResource.Instance ability) {
    var bestScore = 0f;
    ProductionConfig? bestProduction = null;

    foreach (var production in GetPlayerState<IMatchPlayerState>().ProducedEntities) {
      var score = CalculateProductionScore(entity, ability, production);
      if (score > bestScore) {
        bestScore = score;
        bestProduction = production;
      }
    }

    return bestProduction != null
        ? (bestScore, new() { Activator = this, TargetData = new() { Target = bestProduction } })
        : (0, null);
  }

  private float CalculateMoveScore(Entity entity, Entity target, IEnumerable<GameCell> path) {
    var distanceScore = 1f / entity.Cell.Coord.Distance(target.Cell.Coord);
    // var attackScore = CanAttackFrom(entity, path.Last(), target) ? 2f : 0f;
    // var threatScore = CalculateThreatAvoidance(entity, path.Last());

    return distanceScore * 0.6f; //+ attackScore * 0.3f + threatScore * 0.1f;
  }

  private float CalculateAttackScore(Entity attacker, AttackAbilityResource.Instance ability, Entity target) {
    if (attacker.AbilitySystem.AttributeSystem.TryGetAttributeSet<AttackAttributeSet>(out var attackSet) && target.AbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var healthSet)) {
      var damage = attacker.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(attackSet.Damage).GetValueOrDefault();
      var health = target.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(healthSet.HealthAttribute).GetValueOrDefault();

      if (health > 0) {
        return damage / health;
      }
    }

    return 0;
  }

  private float CalculateProductionScore(Entity producer, EntityProductionAbilityResource.Instance ability, ProductionConfig production) {
    if (ability.CanActivateAbility(new() { Activator = this, TargetData = new() { Target = production } })) {
      var forces = EntityManagment.GetOwnedEntites(this);
      // var unitCount = forces.Count(e => e.AbilitySystem.OwnedTags.GetTags().FirstOrDefault(
      //   t =>
      //   t.IsExact(
      //     production.Entity.Tags.GetTags().FirstOrDefault(t => t == TagManager.GetTag(HOBTags.EntityType)))) != null);

      return 1;
    }
    else {
      return 0;
    }
  }

  private IEnumerable<Entity> GetPotentialEnemies(Entity entity) =>
      EntityManagment.GetEnemyEntities(this)
          .Union(EntityManagment.GetNotOwnedEntities());

  public void OwnTurnStarted() {
    var ps = GetPlayerState<IMatchPlayerState>();
    if (ps.AbilitySystem.AttributeSystem.TryGetAttributeSet<PlayerAttributeSet>(out var set)) {
      GD.Print(ps.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.PrimaryResource));
    }
    _ = StartDecisionMaking();
  }
  public void OwnTurnEnded() { }
  public void OnGameStarted() { }
  private void EndTurn() => EndTurnEvent?.Invoke();
  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;
}