namespace GameplayAbilitySystem;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class AttributeSystem : Node {
  [Signal] public delegate void AttributeValueChangedEventHandler(GameplayAttribute attribute, float oldValue, float newValue);

  [Export] private Array<GameplayAttributeSet> AttributeSets { get; set; } = new();

  private Godot.Collections.Dictionary<GameplayAttribute, GameplayAttributeValue> AttributeValues { get; set; } = new();

  public void AddAttributeSet(GameplayAttributeSet attributeSet) {
    AttributeSets.Add(attributeSet);

    foreach (var attribute in attributeSet.GetAttributes()) {
      AttributeValues.TryAdd(attribute, new() { });
    }
  }

  public void RemoveAttributeSet(GameplayAttributeSet attributeSet) {
    AttributeSets.Remove(attributeSet);
    foreach (var attribute in attributeSet.GetAttributes()) {
      AttributeValues.Remove(attribute);
    }
  }

  public IEnumerable<GameplayAttribute> GetAllAttributes() {
    return AttributeValues.Keys;
  }

  public void SetAttributeBaseValue(GameplayAttribute attribute, float baseValue) {
    if (TryGetAttributeValue(attribute, out var value)) {
      if (value != null) {
        var oldValue = value.BaseValue;
        value.BaseValue = baseValue;
        EmitSignal(SignalName.AttributeValueChanged, attribute, oldValue, baseValue);
      }
    }
  }

  public float? GetAttributeCurrentValue(GameplayAttribute attribute) {
    if (TryGetAttributeValue(attribute, out var value)) {
      return value.BaseValue;
    }

    return null;
  }


  public bool TryGetAttributeSet<T>(out T? attributeSet) where T : GameplayAttributeSet {
    attributeSet = AttributeSets.OfType<T>().FirstOrDefault();

    return attributeSet != null;
  }

  public IEnumerable<GameplayAttributeSet> GetAttributeSets() => AttributeSets;

  private bool TryGetAttributeValue(GameplayAttribute attribute, out GameplayAttributeValue? value) {
    return AttributeValues.TryGetValue(attribute, out value);
  }
}