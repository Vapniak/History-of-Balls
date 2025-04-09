namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;

[GlobalClass]
public partial class EntityProductionAbilityResource : HOBAbilityResource {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public partial class Instance : HOBEntityAbilityInstance {
    private int _turnsLeft;

    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override bool CanActivateAbility(GameplayEventData? eventData) {
      if (OwnerEntity.TryGetOwner(out var owner) && eventData?.TargetData?.Target is ProductionConfig productionConfig) {
        return base.CanActivateAbility(eventData) && CheckCostFor(productionConfig.CostEffect, OwnerAbilitySystem, owner.GetPlayerState().AbilitySystem);
      }

      return base.CanActivateAbility(eventData);
    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (OwnerEntity.TryGetOwner(out var owner) && eventData?.TargetData?.Target is ProductionConfig productionConfig) {
        if (productionConfig.CostEffect != null) {
          var ei = OwnerAbilitySystem.MakeOutgoingInstance(productionConfig.CostEffect, 0, owner.GetPlayerState().AbilitySystem);
          ei.Target.ApplyGameplayEffectToSelf(ei);
        }

        // if (owner is PlayerController) {
        //   var text = FloatingText.Create();
        //   text.Label?.AppendText("Start Producing");

        //   GameInstance.GetWorld().AddChild(text);
        //   text.GlobalPosition = OwnerEntity.GetPosition() + Vector3.Up * 2;
        //   _ = text.Animate();
        // }

        _turnsLeft = (int)productionConfig.ProductionTime;

        var tween = CreateTween();
        tween.SetLoops();
        tween.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.OutIn);
        tween.TweenProperty(OwnerEntity.Body, "scale", new Vector3(1.1f, 0.9f, 1.1f), 0.5f).SetDelay(0.1f);
        tween.TweenProperty(OwnerEntity.Body, "scale", new Vector3(0.9f, 1.1f, 0.9f), 0.5f).SetDelay(0.1f);

        var taskData = new WaitForGameplayEventTask(this, TagManager.GetTag(HOBTags.EventTurnStarted));
        taskData.EventRecieved += (data) => {
          if (OwnerEntity.TryGetOwner(out var owner) && OwnerEntity.IsCurrentTurn()) {
            _turnsLeft--;
            if (_turnsLeft <= 0) {
              if (OwnerEntity.EntityManagment.TryAddEntityOnCell(productionConfig.Entity, OwnerEntity.Cell, owner)) {
                taskData.Complete();

                // if (owner is PlayerController) {
                //   var text = FloatingText.Create();
                //   text.Label?.AppendText("Entity produced");
                //   GameInstance.GetWorld().AddChild(text);
                //   text.GlobalPosition = OwnerEntity.GetPosition() + Vector3.Up * 2;
                //   _ = text.Animate();
                // }

                tween.Kill();

                var t = CreateTween();
                t.TweenProperty(OwnerEntity.Body, "scale", new Vector3(1.2f, 1, 1.2f), 0.2f).SetTrans(Tween.TransitionType.Back);
                t.TweenProperty(OwnerEntity.Body, "scale", Vector3.One * 0.8f, 0.1f).SetTrans(Tween.TransitionType.Back);
                t.TweenProperty(OwnerEntity.Body, "scale", Vector3.One, 0.2f).SetTrans(Tween.TransitionType.Back);
                t.TweenProperty(OwnerEntity.Body, "scale", Vector3.One, 0.1f);

                EndAbility();
              }
            }
          }
        };
      }
    }
  }
}
