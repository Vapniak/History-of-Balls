namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;

[GlobalClass]
public partial class EntityProductionAbilityResource : HOBAbilityResource {
  [Export] public ProductionConfig? ProductionConfig { get; private set; }
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public partial class Instance : HOBEntityAbilityInstance {
    private int _turnsLeft;

    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override bool CanActivateAbility(GameplayEventData? eventData) {
      if (OwnerEntity.TryGetOwner(out var owner)) {
        return base.CanActivateAbility(eventData) && CheckCostFor((AbilityResource as EntityProductionAbilityResource).ProductionConfig.CostEffect, OwnerAbilitySystem, owner.GetPlayerState().AbilitySystem);
      }

      return false;
    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (OwnerEntity.TryGetOwner(out var owner)) {
        var ei = OwnerAbilitySystem.MakeOutgoingInstance((AbilityResource as EntityProductionAbilityResource).ProductionConfig.CostEffect, 0);
        ei.Target = owner.GetPlayerState().AbilitySystem;
        ei.Target.ApplyGameplayEffectToSelf(ei);

        if (owner is PlayerController) {
          var text = FloatingText.Create("Start Producing", Colors.Orange);
          GameInstance.GetWorld().AddChild(text);
          text.GlobalPosition = OwnerEntity.GetPosition() + Vector3.Up * 2;
          _ = text.Animate();
        }


        _turnsLeft = (int)(AbilityResource as EntityProductionAbilityResource).ProductionConfig.ProductionTime;

        var taskData = new WaitForGameplayEventTask(this, TagManager.GetTag(HOBTags.EventTurnStarted));
        taskData.EventRecieved += (data) => {
          if (OwnerEntity.TryGetOwner(out var owner) && OwnerEntity.IsCurrentTurn()) {
            _turnsLeft--;
            if (_turnsLeft <= 0) {
              var entity = owner.GetPlayerState().GetEntity((AbilityResource as EntityProductionAbilityResource).ProductionConfig.EntityTag);
              if (OwnerEntity.EntityManagment.TryAddEntityOnCell(entity, OwnerEntity.Cell, owner)) {
                taskData.Complete();

                if (owner is PlayerController) {
                  var text = FloatingText.Create("Entity produced", Colors.Orange);
                  GameInstance.GetWorld().AddChild(text);
                  text.GlobalPosition = OwnerEntity.GetPosition() + Vector3.Up * 2;
                  _ = text.Animate();
                }

                EndAbility(CurrentEventData);
              }
            }
          }
        };
      }
    }
  }
}