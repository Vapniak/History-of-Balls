namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;

[GlobalClass]
public partial class IncomeAbilityResource : HOBAbilityResource {
  [Export] public GameplayEffectResource? IncomeEffect { get; private set; }
  public override GameplayAbilityInstance CreateInstance(GameplayAbilitySystem abilitySystem) {
    return new Instance(this, abilitySystem);
  }


  public partial class Instance : HOBEntityAbilityInstance {
    public Instance(HOBAbilityResource abilityResource, GameplayAbilitySystem abilitySystem) : base(abilityResource, abilitySystem) {

    }
    public override void ActivateAbility(GameplayEventData? eventData) {
      base.ActivateAbility(eventData);

      if (OwnerEntity.TryGetOwner(out var owner)) {
        var income = (AbilityResource as IncomeAbilityResource)?.IncomeEffect;
        if (income != null) {
          var ei = OwnerAbilitySystem.MakeOutgoingInstance(income, 0, owner?.GetPlayerState().AbilitySystem);
          ei.Target.ApplyGameplayEffectToSelf(ei);

          var tween = CreateTween();

          ExecuteGameplayCue(TagManager.GetTag(HOBTags.GameplayCueSparkles), new() { Position = OwnerEntity.Cell.GetRealPosition() });
          tween.TweenProperty(OwnerEntity.Body, "scale", new Vector3(1.2f, 1, 1.2f), 0.2f).SetTrans(Tween.TransitionType.Back);
          tween.TweenProperty(OwnerEntity.Body, "scale", Vector3.One * 0.8f, 0.1f).SetTrans(Tween.TransitionType.Back);
          tween.TweenProperty(OwnerEntity.Body, "scale", Vector3.One, 0.2f).SetTrans(Tween.TransitionType.Back);
          // FIXME: for now check like that
          // if (owner is PlayerController) {
          //   var text = FloatingText.Create();
          //   text.Label?.AppendText("Income Generated");
          //   GameInstance.GetWorld().AddChild(text);
          //   text.GlobalPosition = OwnerEntity.GetPosition() + Vector3.Up * 2;
          //   _ = text.Animate();
          // }
        }
      }

      EndAbility();
    }
  }
}
