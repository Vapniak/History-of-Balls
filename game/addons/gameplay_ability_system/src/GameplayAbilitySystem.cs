namespace GameplayAbilitySystem;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GameplayAbilitySystem : Node {
  [Export] private Array<GameplayAttributeSet> InitialAttributes { get; set; }
  [Export] private Array<GameplayAbility> InitialAbilities { get; set; }

  public GameplayAbilityOwnerInfo OwnerInfo { get; private set; }

  private List<GameplayAttributeSet> SpawnedAttributes { get; set; } = new();
  private List<GameplayAbilityInstance> GrantedAbilities { get; set; } = new();


  public override void _Ready() {
    AddAttributeSet(InitialAttributes);

    if (InitialAbilities != null) {
      foreach (var ability in InitialAbilities) {
        GiveAbility(ability, 0);
      }
    }
  }

  public void InitAbilityOwnerInfo(Node ownerNode) {
    OwnerInfo = new(ownerNode, this);
  }

  public void GiveAbility(GameplayAbility ability, int level) {
    GrantedAbilities.Add(new(ability, level));
  }

  public void RemoveAbility(GameplayAbility ability) {
    GrantedAbilities.Remove(GrantedAbilities.First(e => e.Ability == ability));
  }

  public void AddAttributeSet(IEnumerable<GameplayAttributeSet> attributeSets) {
    if (attributeSets == null) {
      return;
    }

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

    TryActivateAbility<GameplayAbility>(payload: new());
  }

  public bool TryGetAttributeSet<T>(out T attributeSet) where T : GameplayAttributeSet {
    attributeSet = SpawnedAttributes.OfType<T>().FirstOrDefault();
    return attributeSet != null;
  }

  public bool TryActivateAbility<T>(GameplayEventData payload) where T : GameplayAbility {
    var ability = GrantedAbilities.FirstOrDefault(a => a.Ability.GetType() == typeof(T));
    return TryActivateAbility(ability, payload);
  }

  public bool TryActivateAbility(GameplayAbilityInstance instance, GameplayEventData payload) {
    if (instance != null && instance.Ability.TryActivateAbility(instance, OwnerInfo, payload)) {
      return true;
    }

    return false;
  }

  public bool TryGetAbility<T>(out T ability) where T : GameplayAbility {
    ability = GrantedAbilities.FirstOrDefault(a => a.Ability.GetType() == typeof(T)).Ability as T;
    return ability != null;
  }

  public void SendEventData(GameplayEventData data) {
    foreach (var ability in GrantedAbilities) {
      if (ability.Ability.ShouldAbilityRespondToEvent(OwnerInfo, data)) {
        TryActivateAbility(ability, data);
      }
    }
  }
}
