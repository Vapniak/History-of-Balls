namespace GameplayTags;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

public static class TagManager {
  public static readonly Dictionary<string, Tag> Tags = new();
  private static readonly Dictionary<Enum, Tag> _enumToTagMap = new();

  static TagManager() {
    Initialize();
  }
  public static void Initialize() {
    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    foreach (var assembly in assemblies) {
      foreach (var type in assembly.GetTypes()) {
        if (type.IsEnum) {
          RegisterEnumTags(type);
        }
      }
    }

    BuildHierarchy();
  }

  private static void RegisterEnumTags(Type enumType) {
    foreach (var name in Enum.GetNames(enumType)) {
      var field = enumType.GetField(name);
      if (field?.GetCustomAttribute<GameplayTagAttribute>() is GameplayTagAttribute fieldAttr) {
        var fullTagName = $"{fieldAttr.TagName}";

        RegisterHierarchicalTags(fullTagName);

        var enumValue = (Enum)Enum.Parse(enumType, name);
        _enumToTagMap[enumValue] = Tags[fullTagName];
      }
    }
  }

  private static void RegisterHierarchicalTags(string fullTagName) {
    var parts = fullTagName.Split('.');
    var currentPath = "";

    foreach (var part in parts) {
      currentPath = string.IsNullOrEmpty(currentPath) ? part : $"{currentPath}.{part}";

      if (!Tags.ContainsKey(currentPath)) {
        var tag = new Tag(currentPath);
        Tags[currentPath] = tag;
      }
    }
  }

  private static void BuildHierarchy() {
    foreach (var tag in Tags.Values) {
      var parts = tag.FullName.Split('.');
      if (parts.Length > 1) {
        var parentFullName = string.Join(".", parts.Take(parts.Length - 1));
        if (Tags.TryGetValue(parentFullName, out var parentTag)) {
          tag.Parent = parentTag;
          parentTag.AddChild(tag);
        }
      }
    }
  }

  public static Tag GetTag(string fullName) {
    if (Tags.TryGetValue(fullName, out var tag)) {
      return tag;
    }
    GD.PushError($"Tag not found: {fullName}");
    return Tag.Empty;
  }

  public static Tag GetTag(Enum enumValue) {
    if (_enumToTagMap.TryGetValue(enumValue, out var tag)) {
      return tag;
    }
    GD.PushError($"Tag not found for enum value {enumValue}");
    return Tag.Empty;
  }
}
