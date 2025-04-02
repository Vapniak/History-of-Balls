namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System.Threading.Tasks;

[GlobalClass]
public partial class CaptureAbilityResource : HOBAbilityResource {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Ability(this, abilitySystem);
  }

  public partial class Ability : HOBEntityAbilityInstance {
    private WaitForGameplayEventTask? Task { get; set; }

    public Ability(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem)
        : base(abilityResource, abilitySystem) {

    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (eventData != null && eventData.TargetData?.Target != null) {
        _ = StartCapture();
      }
      else {
        EndAbility();
      }
    }

    public override void EndAbility(bool wasCanceled = false) {
      base.EndAbility();

      Task?.Cancel();
    }

    private async Task StartCapture() {
      Task = new WaitForGameplayEventTask(this, TagManager.GetTag(HOBTags.EventTurnStarted));

      Task.EventRecieved += (data) => {
        if (OwnerEntity.IsCurrentTurn()) {
          var target = CurrentEventData?.TargetData?.Target;
          if (OwnerEntity.TryGetOwner(out var owner) && target is Entity entity) {
            entity.ChangeOwner(owner);
            Task.Complete();
            EndAbility();
            return;
          }
        }
      };
    }
  }
}
