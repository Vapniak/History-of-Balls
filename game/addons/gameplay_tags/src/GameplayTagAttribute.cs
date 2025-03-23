namespace GameplayTags;

using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class GameplayTagAttribute : Attribute {
  public string TagName { get; }

  public GameplayTagAttribute(string tagName) {
    TagName = tagName;
  }
}