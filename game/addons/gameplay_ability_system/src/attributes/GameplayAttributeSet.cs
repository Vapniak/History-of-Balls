namespace GameplayAbilitySystem;

using Godot;
using Godot.Collections;

[GlobalClass]
public abstract partial class GameplayAttributeSet : Resource {

  // for now just add basic array, later add c# attribute which is attribute to get all attributes which are defined and get attribute sets by type
  public abstract GameplayAttribute[] GetAttributes();
}