namespace HOB;

using System;
using System.Collections.Generic;
using System.Linq;
using GameplayAbilitySystem;
using GameplayFramework;
using GameplayTags;
using Godot;
using Godot.Collections;
using HOB.GameEntity;

[GlobalClass]
public partial class HOBPlayerState : PlayerState, IMatchPlayerState {
  public Country? Country { get; set; }
  public HOBGameplayAbilitySystem AbilitySystem { get; set; }

  public Array<ProductionConfig> ProducedEntities { get; set; }

  public HOBPlayerState(PlayerAttributeSet playerAttributeSet, Array<ProductionConfig> producedEntities) : base() {
    ProducedEntities = producedEntities;

    AbilitySystem = new();
    AbilitySystem.AttributeSystem.AddAttributeSet(playerAttributeSet);
    AddChild(AbilitySystem);
    AbilitySystem.Owner = this;

    AbilitySystem.AttributeSystem.AttributeValueChanged += OnAttributeValueChanged;
  }

  public bool IsCurrentTurn() => GetController<IMatchController>().IsCurrentTurn();

  private void OnAttributeValueChanged(GameplayAttribute attribute, float oldValue, float newValue) {
    if (AbilitySystem.AttributeSystem.TryGetAttributeSet<PlayerAttributeSet>(out var set)) {
      if ((attribute == set.SecondaryResource)) {
        foreach (var entity in GameInstance.GetGameMode<HOBGameMode>().GetEntityManagment().GetOwnedEntites(GetController<IMatchController>())) {
          entity.AbilitySystem.SendGameplayEvent(TagManager.GetTag(HOBTags.EventResourceGenerated));
        }
      }
    }
  }
}
