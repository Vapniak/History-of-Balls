namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System.Threading.Tasks;

[GlobalClass]
public partial class MeleeAttackAbility : AttackAbility {
  public override GameplayAbility.Instance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public new partial class Instance : AttackAbility.Instance {
    public Instance(HOBAbility abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }
    public override void ActivateAbility(GameplayEventData? eventData) {
      base.ActivateAbility(eventData);

      if (eventData?.TargetData is AttackTargetData attackTargetData && CommitCooldown()) {
        var effect = (AbilityResource as AttackAbility)?.BlockMovementEffect;
        if (effect != null) {
          var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0);
          OwnerAbilitySystem.ApplyGameplayEffectToSelf(ge);
        }

        _ = Attack(attackTargetData.TargetAbilitySystem.GetOwner<Entity>());
        return;
      }

      EndAbility(true);
    }

    public override bool IsCellVisible(GameCell from, GameCell to) {
      return base.IsCellVisible(from, to) && from.GetEdgeTypeTo(to) == GameCell.EdgeType.Flat;
    }

    private async Task Attack(Entity entity) {
      await OwnerEntity.TurnAt(entity.Cell.GetRealPosition(), 0.1f);

      var firstTween = CreateTween();
      firstTween.TweenMethod(Callable.From<Vector3>(OwnerEntity.SetPosition), OwnerEntity.GetPosition(), OwnerEntity.GetPosition() + OwnerEntity.GetPosition().DirectionTo(entity.GetPosition()), .2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

      await ToSignal(firstTween, Tween.SignalName.Finished);

      if (CurrentEventData?.TargetData is AttackTargetData d) {
        var effect = (AbilityResource as AttackAbility)?.DamageEffect;
        if (effect != null) {
          var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0, d.TargetAbilitySystem);
          d.TargetAbilitySystem.ApplyGameplayEffectToSelf(ge);

          if (OwnerEntity.AbilitySystem.OwnedTags.HasTag(TagManager.GetTag(HOBTags.EntityTypeUnitInfantry))) {
            if (!IsInstanceValid(d.TargetAbilitySystem)) {
              CooldownEffectInstance?.QueueFree();
            }

            if (d.TargetAbilitySystem.AttributeSystem.TryGetAttributeSet<HealthAttributeSet>(out var set)) {
              if (d.TargetAbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.HealthAttribute) <= 0) {
                CooldownEffectInstance?.QueueFree();
              }
            }
          }
        }
      }


      var secondTween = CreateTween();
      secondTween.TweenMethod(Callable.From<Vector3>(OwnerEntity.SetPosition), OwnerEntity.GetPosition(), OwnerEntity.Cell.GetRealPosition(), .2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

      await ToSignal(secondTween, Tween.SignalName.Finished);

      base.EndAbility();
    }

    public override uint GetRange() => 1;
  }
}
