namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System.Diagnostics;
using System.Linq;
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
      base.ActivateAbility(eventData);
      if (eventData != null && eventData.TargetData?.Target != null && eventData.TargetData.Target is Entity entity) {
        _ = StartCapture();
      }
      else {
        EndAbility(true);
      }
    }

    public override void EndAbility(bool wasCanceled = false) {
      base.EndAbility(wasCanceled);

      if (!wasCanceled) {
        RemoveBlockTurn();
      }

      Task?.Cancel();
    }

    private async Task StartCapture() {
      Task = new WaitForGameplayEventTask(this, TagManager.GetTag(HOBTags.EventTurnStarted));

      Task.EventRecieved += async (data) => {
        if (OwnerEntity.IsCurrentTurn()) {
          var target = CurrentEventData?.TargetData?.Target;
          if (OwnerEntity.TryGetOwner(out var newOwner) && target is Entity capturedEntity) {
            var typeTag = capturedEntity.AbilitySystem.OwnedTags.GetTags().FirstOrDefault(a => a.IsChildOf(TagManager.GetTag(HOBTags.EntityTypeStructure)));
            if (typeTag == null) {
              Debug.Assert(false, "Type tag is null");
              return;
            }

            var newEntityData = newOwner!.GetPlayerState().Entities.FirstOrDefault(e => e.Tags != null && e.Tags.HasExactTag(typeTag));

            if (newEntityData == null) {
              Debug.Assert(false, "New entity data is null");
              return;
            }

            AddBlockTurn();
            capturedEntity.Die();
            await ToSignal(capturedEntity, SignalName.TreeExited);
            var entityManagment = newOwner.GetGameMode().GetEntityManagment();
            entityManagment.TryAddEntityOnCell(newEntityData, capturedEntity.Cell, newOwner);
            Task.Complete();
            EndAbility();
            return;
          }
        }
      };
    }
  }
}
