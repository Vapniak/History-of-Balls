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

                EndAbility();
              }
            }
          }
        };
      }
    }
  }
}