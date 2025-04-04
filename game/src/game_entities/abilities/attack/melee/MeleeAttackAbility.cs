namespace HOB;

using GameplayAbilitySystem;
using Godot;
using HOB.GameEntity;
using System.Threading.Tasks;

[GlobalClass]
public partial class MeleeAttackAbility : AttackAbility {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public new partial class Instance : AttackAbility.Instance {
    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }
    public override void ActivateAbility(GameplayEventData? eventData) {
      if (eventData?.TargetData is AttackTargetData attackTargetData && CommitCooldown()) {
        var effect = (AbilityResource as AttackAbility)?.BlockMovementEffect;
        if (effect != null) {
          var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0);
          OwnerAbilitySystem.ApplyGameplayEffectToSelf(ge);
        }

        _ = Attack(attackTargetData.TargetAbilitySystem.GetOwner<Entity>());
        return;
      }

      EndAbility();
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
          ShowDamageNumber(entity.Cell.GetRealPosition());
        }
      }


      var secondTween = CreateTween();
      secondTween.TweenMethod(Callable.From<Vector3>(OwnerEntity.SetPosition), OwnerEntity.GetPosition(), OwnerEntity.Cell.GetRealPosition(), .2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

      await ToSignal(secondTween, Tween.SignalName.Finished);

      EndAbility();
    }


    public override uint GetRange() => 1;
  }
}
