namespace HOB;

using GameplayAbilitySystem;
using Godot;
using HOB.GameEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class AttackAbilityResource : HOBAbilityResource {
  [Export] public GameplayEffectResource? DamageEffect { get; private set; }

  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new AttackAbilityInstance(this, abilitySystem);
  }

  public partial class AttackAbilityInstance : HOBAbilityInstance {
    public AttackAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override bool CanActivateAbility(GameplayEventData eventData) {
      if (eventData.TargetData is AttackTargetData data) {
        return GetAttackableEntities().entities.Contains(data.TargetAbilitySystem.GetOwner<Entity>()) && base.CanActivateAbility(eventData);
      }

      return false;
    }

    public override async Task ActivateAbility(GameplayEventData eventData) {
      GD.Print("attacked");
      if (CurrentEventData?.TargetData is AttackTargetData d) {
        var effect = (AbilityResource as AttackAbilityResource)?.DamageEffect;
        if (effect != null) {
          var ge = OwnerAbilitySystem.MakeOutgoingInstance(effect, 0);
          d.TargetAbilitySystem.TryApplyGameplayEffectToSelf(ge);
        }
      }

      if (eventData.TargetData is AttackTargetData data) {
        await Attack(data.TargetAbilitySystem.GetOwner<Entity>());
      }

      await EndAbility(eventData);
    }

    public async Task Attack(Entity entity) {
      await OwnerEntity.TurnAt(entity.Cell.GetRealPosition(), 0.1f);

      var firstTween = CreateTween();
      firstTween.TweenMethod(Callable.From<Vector3>(OwnerEntity.SetPosition), OwnerEntity.GetPosition(), OwnerEntity.GetPosition() + OwnerEntity.GetPosition().DirectionTo(entity.GetPosition()), .2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

      await ToSignal(firstTween, Tween.SignalName.Finished);

      var secondTween = CreateTween();
      secondTween.TweenMethod(Callable.From<Vector3>(OwnerEntity.SetPosition), OwnerEntity.GetPosition(), OwnerEntity.Cell.GetRealPosition(), .2f).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

      await ToSignal(secondTween, Tween.SignalName.Finished);
    }

    public (Entity[] entities, GameCell[] cellsInRange) GetAttackableEntities() {
      var attackableEntities = new List<Entity>();
      var cellsInR = new List<GameCell>();

      if (OwnerEntity.AbilitySystem.TryGetAttributeSet<AttackAttributeSet>(out var attributeSet)) {
        if (attributeSet != null) {
          if (OwnerEntity.AbilitySystem.TryGetAttributeCurrentValue(attributeSet.Range, out var currentValue)) {
            var cells = OwnerAbilitySystem.GetOwner<Entity>().Cell.GetCellsInRange((uint)currentValue.GetValueOrDefault());
            foreach (var cell in cells) {
              if (cell == OwnerEntity.Cell) {
                continue;
              }

              cellsInR.Add(cell);
              var entities = OwnerEntity.EntityManagment.GetEntitiesOnCell(cell);
              attackableEntities.AddRange(entities.Where(CanBeAttacked));
            }
          }
        }
      }

      return (attackableEntities.ToArray(), cellsInR.ToArray());
    }

    public bool CanBeAttacked(Entity entity) {
      return entity.TryGetOwner(out var enemyOwner) && OwnerEntity.TryGetOwner(out var owner) && enemyOwner != owner;
    }
  }
}
