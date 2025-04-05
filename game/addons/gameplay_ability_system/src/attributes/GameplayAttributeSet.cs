namespace GameplayAbilitySystem;

using Godot;
using Godot.Collections;

[GlobalClass]
public abstract partial class GameplayAttributeSet : Resource {
  // for now just add basic array, later add c# attribute which is attribute to get all attributes which are defined and get attribute sets by type

  /// <summary>
  /// Responds to changes to current value
  /// </summary>
  /// <param name="attribute"></param>
  /// <param name="newValue"></param>
  public virtual void PreAttributeChange(GameplayAttribute attribute, ref float newValue) {

  }

  public virtual void PostAttributeChange(GameplayAttribute attribute, float oldValue, float newValue) {

  }

  public virtual void PreGameplayEffectExecute(GameplayEffectModData data) {

  }
  /// <summary>
  /// Responds to changes to base value on instant gameplay effects
  /// </summary>
  public virtual void PostGameplayEffectExecute(GameplayEffectModData data) {

  }
  public abstract GameplayAttribute[] GetAttributes();
}