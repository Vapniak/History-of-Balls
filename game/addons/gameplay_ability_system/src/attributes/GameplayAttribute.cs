namespace GameplayAbilitySystem;

using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class GameplayAttribute : Resource {
  [Export] public string AttributeName { get; private set; }

  public virtual GameplayAttributeValue CalculateCurrentValue(GameplayAttributeValue value, List<GameplayAttributeValue> otherAttributes) {
    return value;
  }
}
