namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class AIController : Controller, IMatchController {
  public Country? Country { get; set; }
  public AIProfile Profile { get; set; } = new();

  private IEntityManagment EntityManagment => GetGameMode().GetEntityManagment();
  private HOBGameMode GameMode => GetGameMode();

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  public async Task StartDecisionMaking() {
    await ProcessEntityActions();

    while (((IMatchController)this).IsCurrentTurn()) {
      await Task.Delay(1000);
      EndTurn();
    }
  }

  private async Task ProcessEntityActions() {
    while (true) {
      var entities = EntityManagment.GetOwnedEntites(this).ToList();
      var batchSize = Mathf.Max(16 / entities.Count, 1);
      if (entities.Count == 0) {
        break;
      }

      var bestScore = 0f;
      Entity? bestEntity = null;
      GameplayAbilityInstance? bestAbility = null;
      GameplayEventData? bestData = null;

      for (var i = 0; i < entities.Count; i += batchSize) {
        var batch = entities
               .Skip(i)
               .Take(batchSize)
               .ToList();


        var results = new ConcurrentBag<(Entity?, GameplayAbilityInstance?, GameplayEventData?, float)>();
        await Task.Run(() =>
          Parallel.ForEach(batch,
          entity => results.Add(EvaluateEntity(entity)))
        );

        var (entity, ability, data, score) = results
            .OrderByDescending(r => r.Item4)
            .FirstOrDefault();

        if (score > bestScore) {
          bestScore = score;
          bestEntity = entity;
          bestAbility = ability;
          bestData = data;
        }

        await Task.Delay(16);
      }

      if (bestAbility == null) {
        break;
      }

      if (bestEntity != null && bestAbility != null) {
        GD.PrintS($"AI attempting: {bestEntity.EntityName} -> {bestAbility.AbilityResource.AbilityName}");

        if (bestAbility is AttackAbility.Instance or MoveAbility.Instance) {
          var success = await bestEntity.AbilitySystem.TryActivateAbilityAsync(bestAbility, bestData);

          if (!success) {
            // GD.PrintS("Activation failed! Re-evaluating...");
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

  private (Entity Entity, GameplayAbilityInstance? Ability, GameplayEventData? Data, float Score)
      EvaluateEntity(Entity entity) {
    var timer = System.Diagnostics.Stopwatch.StartNew();

    var (ability, data, score) = FindBestActionWithBudget(entity);

    timer.Stop();
    // GD.Print($"Processed {entity.EntityName} in {timer.ElapsedMilliseconds}ms");
    return (entity, ability, data, score);
  }


  private (Entity Entity, GameplayAbilityInstance? Ability, GameplayEventData? Data, float Score)
    EvaluateEntityAsync(Entity entity) {
    var timer = System.Diagnostics.Stopwatch.StartNew();
    var (ability, data, score) = FindBestActionWithBudget(entity);
    timer.Stop();
    // GD.Print($"Processed {entity.EntityName} in {timer.ElapsedMilliseconds}ms");
    return (entity, ability, data, score);
  }

  private (GameplayAbilityInstance?, GameplayEventData?, float)
      FindBestActionWithBudget(Entity entity) {
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
      MoveAbility.Instance moveAbility => EvaluateMoveAbility(entity, moveAbility),
      AttackAbility.Instance attackAbility => EvaluateAttackAbility(entity, attackAbility),
      EntityProductionAbilityResource.Instance produceAbility => EvaluateProduceAbility(entity, produceAbility),
      _ => (0, null)
    };
  }

  private (float Score, GameplayEventData? Data) EvaluateMoveAbility(Entity entity, MoveAbility.Instance ability) {
    const int maxEnemiesToConsider = 3;
    var bestScore = 0f;
    GameCell? bestCell = null;

    var enemies = GetPotentialEnemies(entity)
        .OrderBy(e => entity.Cell.Coord.Distance(e.Cell.Coord))
        .Take(maxEnemiesToConsider);

    foreach (var enemy in enemies) {
      var path = ability.FindPathTo(enemy.Cell);
      if (path == null || path.Length == 0) {
        continue;
      }

      foreach (var cell in path) {
        var score = CalculateMoveScore(entity, enemy, path, cell);
        if (score > bestScore) {
          bestScore = score;
          bestCell = cell;
        }
      }
    }

    return bestCell != null
        ? (bestScore, new GameplayEventData() {
          Activator = this,
          TargetData = new MoveTargetData() { Cell = bestCell }
        })
        : (0, null);
  }

  private (float Score, GameplayEventData? Data) EvaluateAttackAbility(Entity entity, AttackAbility.Instance ability) {
    var bestScore = 0f;
    Entity? bestTarget = null;

    var (entities, _) = ability.GetAttackableEntities();
    foreach (var target in entities) {
      var score = CalculateDamageRatio(entity, ability, target);
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

  private float CalculateMoveScore(Entity entity, Entity target, IEnumerable<GameCell> path, GameCell cell) {
    var attackAbility = entity.AbilitySystem.GetGrantedAbility<AttackAbility.Instance>();
    var distanceScore = 1f;
    var attackScore = 1f;
    uint desiredDistance = 0;
    var actualDistance = cell.Coord.Distance(target.Cell.Coord);
    var valueScore = 1f;

    if (attackAbility != null && attackAbility.GetAttackableEntities(cell).entities.Contains(target)) {
      attackScore = CalculateDamageRatio(entity, attackAbility, target) * Profile.Agressiveness;
      var attackRange = attackAbility.GetRange();
      desiredDistance = attackRange;
    }

    valueScore = CalculateValueScore(target) * Profile.Expansiveness;

    // var threatScore = Calcu  lateThreatAvoidance(entity, cell);
    distanceScore = 1f / (Mathf.Abs(actualDistance - desiredDistance) + 1);

    return distanceScore * attackScore * valueScore;
  }

  private float CalculateThreatAvoidance(Entity entity, GameCell cell) {
    var threatCount = 0;
    foreach (var enemy in GetPotentialEnemies(entity)) {
      var enemyAttack = enemy.AbilitySystem.GetGrantedAbility<AttackAbility.Instance>();
      if (enemyAttack != null && enemyAttack.GetAttackableEntities().cellsInRange.Contains(cell)) {
        threatCount++;
      }
    }
    return Mathf.Min(threatCount / 5f, 0.5f);
  }

  private static float CalculateValueScore(Entity target) {
    var tags = target.AbilitySystem.OwnedTags;
    if (tags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureCity))) {
      return .5f;
    }
    else if (tags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureVillage))) {
      return 0.75f;
    }
    else if (tags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructureFactory))) {
      return 1f;
    }

    return 0.1f;
  }

  private static float CalculateDamageRatio(Entity attacker, AttackAbility.Instance ability, Entity target) {
    if (attacker.AbilitySystem.AttributeSystem.TryGetAttributeSet<AttackAttributeSet>(out var attackSet) && target.AbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var healthSet)) {
      var damage = attacker.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(attackSet.Damage).GetValueOrDefault();
      var health = target.AbilitySystem.AttributeSystem.GetAttributeCurrentValue(healthSet.HealthAttribute).GetValueOrDefault();

      if (health > 0) {
        if (damage > health) {
          return health / damage;
        }
        else {
          return damage / health;
        }
      }
    }

    return 0;
  }

  private float CalculateProductionScore(Entity producer, EntityProductionAbilityResource.Instance ability, ProductionConfig production) {
    if (!ability.CanActivateAbility(new() { Activator = this, TargetData = new() { Target = production } })) {
      return 0f;
    }

    var unitTypeTag = TagManager.GetTag(HOBTags.EntityTypeUnit);
    var forces = EntityManagment.GetOwnedEntites(this);

    var units = forces.Where(e => e.AbilitySystem.OwnedTags.HasTag(unitTypeTag)).ToList();

    var subtypeCounts = units
        .Select(u => {
          var tags = u.AbilitySystem.OwnedTags.GetAllTags();
          return new {
            Unit = u,
            SubTypeTag = tags.FirstOrDefault(t => t.IsChildOf(unitTypeTag))
          };
        })
        .Where(x => x.SubTypeTag != null)
        .GroupBy(x => x.SubTypeTag)
        .Select(g => new {
          SubTypeTag = g.Key,
          Count = g.Count(),
          TypeName = g.Key?.Name ?? "NULL"
        })
        .ToList();

    if (subtypeCounts.Count == 0) {
      return 1f;
    }


    var productionTags = production.Entity?.Tags?.GetAllTags();
    foreach (var subtype in subtypeCounts) {
      if (subtype.Count == 0) {
        return 1;
      }

      if (production.Entity?.Tags != null && production.Entity.Tags.HasTag(subtype.SubTypeTag)) {
        return Mathf.Min(1f / subtype.Count, 1);
      }
    }

    return 0f;
  }

  private IEnumerable<Entity> GetPotentialEnemies(Entity entity) =>
      EntityManagment.GetEnemyEntities(this)
          .Union(EntityManagment.GetNotOwnedEntities());
  public void OwnTurnStarted() {
    _ = StartDecisionMaking();
  }
  public void OwnTurnEnded() { }
  public void OnGameStarted() { }
  private void EndTurn() {
    GetGameMode().GetTurnManagment().TryEndTurn(this);
  }
  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;
}
