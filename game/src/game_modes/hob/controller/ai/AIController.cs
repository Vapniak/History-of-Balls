namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using Godot;
using HOB.GameEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

[GlobalClass]
public partial class AIController : Controller, IMatchController {
  public event Action? EndTurnEvent;

  public Country? Country { get; set; }

  private IEntityManagment EntityManagment => GetGameMode().GetEntityManagment();

  public override IMatchGameState GetGameState() => base.GetGameState() as IMatchGameState;

  private void EndTurn() {
    EndTurnEvent?.Invoke();
  }

  public async Task StartDecisionMaking() {
    // FIXME: temp fix for ai ending turn before checking for win condition in game mode
    await Task.Delay(1000);

    // foreach (var entity in EntityManagment.GetOwnedEntites(this)) {
    //   await Decide(entity);
    // }

    EndTurn();
  }

  public void OwnTurnStarted() {
    _ = StartDecisionMaking();
  }

  public async Task Decide(Entity entity) {
    GameplayAbilityInstance? bestAbility = null;
    GameplayEventData? bestEventData = null;
    var bestScore = float.MinValue;

    var abilities = entity.AbilitySystem.GetGrantedAbilities();

    foreach (var ability in abilities) {
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

    if (bestAbility != null) {
      var result = await entity.AbilitySystem.TryActivateAbilityAsync(bestAbility, bestEventData);
      if (!result) {
        GD.PrintErr("Failed to activate ability: ", bestAbility.AbilityResource.AbilityName);
      }
    }
  }

  public void OwnTurnEnded() { }

  public void OnGameStarted() { }

  IMatchPlayerState IMatchController.GetPlayerState() => base.GetPlayerState() as IMatchPlayerState;
  public new HOBGameMode GetGameMode() => base.GetGameMode() as HOBGameMode;

  private async Task<(float score, GameplayEventData? data)> EvaluateAbility(Entity entity, GameplayAbilityInstance ability) {
    if (ability is MoveAbilityResource.MoveAbilityInstance moveAbility) {
      return await EvaluateMoveAbility(entity, moveAbility);
    }

    return (0, null);
  }

  private async Task<(float Score, GameplayEventData? Data)> EvaluateMoveAbility(Entity entity, MoveAbilityResource.MoveAbilityInstance moveAbility) {
    var bestScore = float.MinValue;
    GameCell? bestCell = null;

    var enemyEntities = EntityManagment.GetEnemyEntities(this)
        .Union(EntityManagment.GetNotOwnedEntities())
        .Where(e => e != null);

    foreach (var enemy in enemyEntities) {
      var path = moveAbility.FindPathTo(enemy.Cell);
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
        ? (bestScore, new GameplayEventData {
          Activator = this,
          TargetData = new MoveTargetData { Cell = bestCell }
        })
        : (0, null);
  }

  private float CalculateMoveScore(Entity entity, Entity enemy, IEnumerable<GameCell> path) {
    // Calculate distance score (closer is better)
    var distanceScore = 1f / (path.Count() + 1);

    // Calculate threat score (avoid clustering)
    // var nearbyAllies = EntityManagment.GetEntitiesInRadius(entity.Cell, 3)
    //     .Count(e => e != entity && e.OwnerController == this);
    // var spreadScore = 1f / (nearbyAllies + 1);

    // Calculate attack position score
    // var attackScore = CanAttackFromPosition(entity, path.Last()) ? 2f : 0f;

    // Combine scores with weights
    return distanceScore;// * 0.6f + spreadScore * 0.3f + attackScore * 1f;
  }
}
