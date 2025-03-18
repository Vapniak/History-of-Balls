namespace GameplayAbilitySystem;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GameplayAbilitySystem : Node {
  [Export] private Array<GameplayAttributeSet> InitialAttributes { get; set; }
  private List<GameplayAttributeSet> SpawnedAttributes { get; set; } = new();

  public override void _Ready() {
    AddAttributeSet(InitialAttributes);
  }

  public void AddAttributeSet(IEnumerable<GameplayAttributeSet> attributeSets) {
    foreach (var attributeSet in attributeSets) {
      AddAttributeSet(attributeSet);
    }
  }
  public void AddAttributeSet(GameplayAttributeSet attributeSet) {
    if (attributeSet != null) {
      SpawnedAttributes.Add(attributeSet.Duplicate() as GameplayAttributeSet);
    }
  }

  public void RemoveAttributeSet(GameplayAttributeSet attributeSet) {
    SpawnedAttributes.Remove(attributeSet);
  }

  public void RemoveAttributeSet<T>() where T : GameplayAttributeSet {
    TryGetAttributeSet(out T attributeSet);
    SpawnedAttributes.Remove(attributeSet);
  }

  public bool TryGetAttributeSet<T>(out T attributeSet) where T : GameplayAttributeSet {
    attributeSet = SpawnedAttributes.OfType<T>().FirstOrDefault();
    return attributeSet != null;
  }
}