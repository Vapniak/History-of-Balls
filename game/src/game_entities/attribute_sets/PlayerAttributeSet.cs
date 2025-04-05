namespace HOB;

using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using System;

[GlobalClass]
public partial class PlayerAttributeSet : GameplayAttributeSet {
  [Export] public GameplayAttribute PrimaryResource { get; private set; } = new();
  [Export] public GameplayAttribute SecondaryResource { get; private set; } = new();

  public override void PostGameplayEffectExecute(GameplayEffectModData data) {
    base.PostGameplayEffectExecute(data);

    var @as = data.EffectInstance.Target;
    foreach (var entity in GameInstance.GetGameMode<HOBGameMode>().GetEntityManagment().GetOwnedEntites(@as.GetOwner<HOBPlayerState>().GetController<IMatchController>())) {
      entity.AbilitySystem.SendGameplayEvent(TagManager.GetTag(HOBTags.EventResourceGenerated));
    }
  }
  public override GameplayAttribute[] GetAttributes() => new[] { PrimaryResource, SecondaryResource };
}
