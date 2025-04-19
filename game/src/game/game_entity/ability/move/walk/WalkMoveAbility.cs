namespace HOB;

using AudioManager;
using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public partial class WalkMoveAbility : MoveAbility {
  [Export] public int MaxWalkElevation = 8;

  public override GameplayAbility.Instance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public new partial class Instance : MoveAbility.Instance {
    public Instance(MoveAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      base.ActivateAbility(eventData);

      if (eventData?.TargetData is MoveTargetData moveTargetData && CommitCooldown()) {
        _ = WalkByPath(moveTargetData.Cell);
      }
      else {
        EndAbility(true);
      }
    }

    private async Task WalkByPath(GameCell to) {
      var path = FindPathTo(to);

      if (path.Length > 0 && to != OwnerEntity.Cell) {
        var originalScale = OwnerEntity.Body.Scale;
        OwnerEntity.Cell = path.Last();
        foreach (var cell in path) {
          const float JUMP_HEIGHT = 1.5f;
          const float SQUASH_SCALE = 0.8f;
          const float STRETCH_SCALE = 1.2f;
          const float ANTICIPATION_DURATION = MOVE_ANIMATION_SPEED / 2f;

          var startPosition = OwnerEntity.Position;
          var targetPosition = cell.GetRealPosition();
          var tween = OwnerEntity.CreateTween();

          if (cell == path.First()) {
            tween.TweenProperty(OwnerEntity.Body, "scale",
                new Vector3(originalScale.X * STRETCH_SCALE, originalScale.Y * SQUASH_SCALE, originalScale.Z * STRETCH_SCALE),
                ANTICIPATION_DURATION)
                .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

            await ToSignal(tween, Tween.SignalName.Finished);
            tween.Kill();
          }
          else {
            tween.TweenProperty(OwnerEntity.Body, "scale",
                new Vector3(originalScale.X, originalScale.Y * STRETCH_SCALE, originalScale.Z),
                ANTICIPATION_DURATION)
                .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

            await ToSignal(tween, Tween.SignalName.Finished);
          }

          tween = OwnerEntity.CreateTween();

          var verticalDifference = targetPosition.Y - startPosition.Y;
          var horizontalDistance = new Vector2(
              targetPosition.X - startPosition.X,
              targetPosition.Z - startPosition.Z
          ).Length();

          var peakHeight = Mathf.Max(startPosition.Y, targetPosition.Y) + JUMP_HEIGHT;
          var jumpDuration = MOVE_ANIMATION_SPEED * Mathf.Max(1, horizontalDistance / 3f);

          tween.Parallel().TweenMethod(Callable.From<float>(t => {
            var horizontalProgress = t;
            var verticalProgress = (-4 * (t - 0.5f) * (t - 0.5f)) + 1;

            var currentY = Mathf.Lerp(startPosition.Y, targetPosition.Y, horizontalProgress)
                         + (verticalProgress * (peakHeight - Mathf.Max(startPosition.Y, targetPosition.Y)));

            OwnerEntity.Position = new Vector3(
                Mathf.Lerp(startPosition.X, targetPosition.X, horizontalProgress),
                currentY,
                Mathf.Lerp(startPosition.Z, targetPosition.Z, horizontalProgress)
            );
          }), 0f, 1f, jumpDuration)
          .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);

          tween.Parallel().TweenProperty(OwnerEntity.Body, "scale",
              new Vector3(originalScale.X * STRETCH_SCALE, originalScale.Y * SQUASH_SCALE, originalScale.Z),
              MOVE_ANIMATION_SPEED)
              .SetEase(Tween.EaseType.Out);

          _ = OwnerEntity.TurnAt(targetPosition, MOVE_ANIMATION_SPEED);

          await ToSignal(tween, Tween.SignalName.Finished);
          // landed
          var pitch = GD.RandRange(2, 3);

          ExecuteGameplayCue(TagManager.GetTag(HOBTags.GameplayCueMoveDust), new() { Position = cell.GetRealPosition() });

          SoundManager.Instance.PlayVaried("sounds", "tick", pitch);



          tween = CreateTween();

          tween.TweenProperty(OwnerEntity.Body, "scale",
                  new Vector3(originalScale.X * STRETCH_SCALE, originalScale.Y * SQUASH_SCALE / 2f, originalScale.Z * STRETCH_SCALE),
                  MOVE_ANIMATION_SPEED / 2f)
              .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Circ);

          if (cell == path.Last()) {
            tween
                .TweenProperty(OwnerEntity.Body, "scale", originalScale * STRETCH_SCALE, MOVE_ANIMATION_SPEED)
                .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

            tween
                .TweenProperty(OwnerEntity.Body, "scale", originalScale, MOVE_ANIMATION_SPEED / 2f)
                .SetEase(Tween.EaseType.In);
          }

          await ToSignal(tween, Tween.SignalName.Finished);

          OwnerEntity.Position = targetPosition;
        }

        var strucure = OwnerEntity.EntityManagment.GetEntitiesOnCell(path.Last()).FirstOrDefault(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure)));

        if (strucure != null) {
          strucure.TryGetOwner(out var owner2);
          if (OwnerEntity.TryGetOwner(out var owner) && owner != null && owner2 != owner) {
            OwnerAbilitySystem.SendGameplayEvent(TagManager.GetTag(HOBTags.EventEntityCapture), new() { Activator = owner, TargetData = new() { Target = strucure } });
          }
        }
      }

      EndAbility();
    }

    protected override bool IsReachable(GameCell from, GameCell to) => base.IsReachable(from, to) && to.GetSetting().Elevation <= ((WalkMoveAbility)AbilityResource).MaxWalkElevation;
  }
}
