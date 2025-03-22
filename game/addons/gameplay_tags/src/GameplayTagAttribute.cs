// GameplayTagAttribute.cs
namespace GameplayTags;

using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class GameplayTagAttribute : Attribute {
  public string TagName { get; }

  public GameplayTagAttribute(string tagName) {
    TagName = tagName;
  }
}

public enum TestGameplayTags {
  [GameplayTag("Ability.Fire.Damage")] FireDameage,
  [GameplayTag("Ability.Ice")] Ice,
  [GameplayTag("Ability.Heal")] Heal,
  [GameplayTag("Character.Player")] Player,
  [GameplayTag("Character.Enemy")] Enemy
}