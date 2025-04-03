namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class WalkMoveAbility : MoveAbility {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public new partial class Instance : MoveAbility.Instance {
    public Instance(MoveAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (eventData?.TargetData is MoveTargetData moveTargetData && CommitCooldown()) {
        _ = WalkByPath(moveTargetData.Cell);
      }
      else {
        EndAbility();
      }
    }

    private async Task WalkByPath(GameCell to) {
      var path = FindPathTo(to);

      if (path.Length > 0 && to != OwnerEntity.Cell) {
        foreach (var cell in path) {
          await Walk(cell);
        }

        var strucure = OwnerEntity.EntityManagment.GetEntitiesOnCell(path.Last()).FirstOrDefault(e => e.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeStructure)));

        if (strucure != null && OwnerEntity.TryGetOwner(out var owner) && owner != null) {
          OwnerAbilitySystem.SendGameplayEvent(TagManager.GetTag(HOBTags.EventEntityCapture), new() { Activator = owner, TargetData = new() { Target = strucure } });
        }
      }

      EndAbility();
    }

    private async Task Walk(GameCell to) {
      var startPosition = OwnerEntity.GetPosition();
      var targetPosition = to.GetRealPosition();

      var midpoint = (startPosition + targetPosition) / 2;
      midpoint.Y += 1.0f;

      var tween = OwnerEntity.CreateTween();

      tween.TweenMethod(
          Callable.From<Vector3>(OwnerEntity.SetPosition),
          startPosition,
          midpoint,
          MOVE_ANIMATION_SPEED
      ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);

      OwnerEntity.Cell = to;

      OwnerEntity.TurnAt(targetPosition, MOVE_ANIMATION_SPEED);


      tween.TweenMethod(
          Callable.From<Vector3>(OwnerEntity.SetPosition),
          midpoint,
          targetPosition,
          MOVE_ANIMATION_SPEED
      ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

      await ToSignal(tween, Tween.SignalName.Finished);
    }
  }
}
