namespace GameplayAbilitySystem;

using GameplayTags;
using Godot;
using System;

[GlobalClass, Tool]
public partial class SetByCallerModiferMagnitudeResource : ModifierMagnitudeResource {
  [Export] public Tag? DataTag;

  public override float CalculateMagnitude(GameplayEffectInstance effectInstance) {
    if (DataTag == null) {
      return 1;
    }

    return effectInstance.GetSetByCallerMagnitude(DataTag);
  }
  public override void Initialize(GameplayEffectInstance effectInstance) {

  }
}
