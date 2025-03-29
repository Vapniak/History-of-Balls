namespace HOB;

using GameplayAbilitySystem;
using GameplayTags;
using Godot;
using HOB.GameEntity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public partial class CaptureAbilityResource : HOBAbilityResource {
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new CaptureAbilityInstance(this, abilitySystem);
  }

  public partial class CaptureAbilityInstance : HOBEntityAbilityInstance {
    private Task? _captureTask;
    private readonly CancellationTokenSource _cts;

    public CaptureAbilityInstance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem)
        : base(abilityResource, abilitySystem) {
      _cts = new CancellationTokenSource();
    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (eventData != null && eventData.TargetData?.Target != null) {
        _captureTask = StartCapture(_cts.Token);
      }
      else {
        EndAbility(eventData);
      }
    }

    public override void EndAbility(GameplayEventData? eventData) {
      base.EndAbility(eventData);

      _cts.Cancel();
      _cts.Dispose();
      _captureTask = null;
    }

    private async Task StartCapture(CancellationToken ct) {
      try {
        while (!ct.IsCancellationRequested) {
          var awaiter = ToSignal(OwnerAbilitySystem, nameof(OwnerAbilitySystem.GameplayEventRecieved));
          await awaiter;

          var tag = awaiter.GetResult().First(e => e.As<Resource>() is Tag);
          if ((tag.As<Resource>() as Tag) == TagManager.GetTag(HOBTags.EventTurnStarted) &&
              OwnerEntity.IsCurrentTurn()) {
            var target = CurrentEventData?.TargetData?.Target;
            if (OwnerEntity.TryGetOwner(out var owner) && target is Entity entity) {
              entity.ChangeOwner(owner);
              EndAbility(CurrentEventData);
              return;
            }
          }
        }
      }
      catch (OperationCanceledException) {

      }
    }
  }
}
