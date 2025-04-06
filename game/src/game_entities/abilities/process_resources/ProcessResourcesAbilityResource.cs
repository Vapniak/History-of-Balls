namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;

[GlobalClass]
public partial class ProcessResourcesAbilityResource : HOBAbilityResource {
  [Export] public GameplayEffectResource? ProduceEffect { get; private set; }
  [Export] public GameplayEffectResource? ProcessEffect { get; private set; }

  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }

  public partial class Instance : HOBEntityAbilityInstance {
    private int _turnsLeft;

    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }

    public override bool CanActivateAbility(GameplayEventData? eventData) {
      OwnerEntity.TryGetOwner(out var owner);
      if (owner == null) {
        return false;
      }

      var abilitySystem = owner?.GetPlayerState().AbilitySystem;
      if (abilitySystem == null) {
        return false;
      }

      return base.CanActivateAbility(eventData) && CheckCostFor((AbilityResource as ProcessResourcesAbilityResource).ProcessEffect, OwnerAbilitySystem, abilitySystem);
    }

    public override void ActivateAbility(GameplayEventData? eventData) {
      if (OwnerEntity.TryGetOwner(out var owner)) {
        var abilitySystem = owner?.GetPlayerState().AbilitySystem;
        var processEffect = OwnerAbilitySystem.MakeOutgoingInstance((AbilityResource as ProcessResourcesAbilityResource).ProcessEffect, 0, abilitySystem);
        processEffect.Target.ApplyGameplayEffectToSelf(processEffect);

        // if (owner is PlayerController) {
        //   var text = FloatingText.Create();
        //   text.Label?.AppendText("");
        //   GameInstance.GetWorld().AddChild(text);
        //   text.GlobalPosition = OwnerEntity.GetPosition() + Vector3.Up * 2;
        //   _ = text.Animate();
        // }

        if (OwnerAbilitySystem.AttributeSystem.TryGetAttributeSet<ProcessResourcesAttributeSet>(out var set)) {
          _turnsLeft = (int)OwnerAbilitySystem.AttributeSystem.GetAttributeCurrentValue(set.ProcessTime);
        }
        Process();
      }
    }

    private void Process() {
      var taskData = new WaitForGameplayEventTask(this, TagManager.GetTag(HOBTags.EventTurnStarted));
      var tween = CreateTween();
      tween.SetLoops();
      tween.SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.OutIn);
      tween.TweenProperty(OwnerEntity.Body, "scale", new Vector3(1.1f, 0.9f, 1.1f), 0.5f).SetDelay(0.1f);
      tween.TweenProperty(OwnerEntity.Body, "scale", new Vector3(0.9f, 1.1f, 0.9f), 0.5f).SetDelay(0.1f);
      taskData.EventRecieved += (data) => {
        if (OwnerEntity.TryGetOwner(out var owner) && OwnerEntity.IsCurrentTurn()) {
          _turnsLeft--;
          if (_turnsLeft <= 0) {
            var abilitySystem = owner?.GetPlayerState().AbilitySystem;
            var produceEffect = OwnerAbilitySystem.MakeOutgoingInstance((AbilityResource as ProcessResourcesAbilityResource).ProduceEffect, 0, abilitySystem);

            produceEffect.Target.ApplyGameplayEffectToSelf(produceEffect);
            taskData.Complete();

            // if (owner is PlayerController) {
            //   var text = FloatingText.Create();
            //   text.Label?.AppendText("Processing Finished");
            //   GameInstance.GetWorld().AddChild(text);
            //   text.GlobalPosition = OwnerEntity.GetPosition() + Vector3.Up * 2;
            //   _ = text.Animate();
            // }

            tween.Kill();

            var t = CreateTween().SetTrans(Tween.TransitionType.Back);
            t.TweenProperty(OwnerEntity.Body, "scale", new Vector3(1.2f, 1, 1.2f), 0.2f);
            t.TweenProperty(OwnerEntity.Body, "scale", Vector3.One * 0.8f, 0.1f);
            t.TweenProperty(OwnerEntity.Body, "scale", Vector3.One, 0.2f);

            EndAbility();
          }
        }
      };
    }
  }
}
