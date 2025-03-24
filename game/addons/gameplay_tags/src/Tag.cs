namespace GameplayTags;

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass, Tool]
public partial class Tag : Resource, IEquatable<Tag> {
  public static readonly Tag Empty = new(string.Empty);

  [Export] public string FullName { get; set; } = string.Empty;

  public string Name { get; } = string.Empty;
  public Tag? Parent { get; set; }

  private readonly HashSet<Tag> _children = new();

  public Tag() { }

  public Tag(string fullName) {
    FullName = fullName;
    var parts = fullName.Split('.');
    Name = parts[^1];

    Parent = parts.Length > 1 ? new Tag(string.Join(".", parts.Take(parts.Length - 1))) : Empty;

    _children = new HashSet<Tag>();
  }

  public void AddChild(Tag child) {
    _children.Add(child);
  }

  public IEnumerable<Tag> GetChildren() {
    return _children;
  }

  public bool Equals(Tag? other) {
    if (other?.FullName != null) {
      return FullName.StartsWith(other.FullName);
    }
    return false;
  }

  public override bool Equals(object? obj) {
    return Equals(obj as Tag);
  }

  public override int GetHashCode() {
    return FullName?.GetHashCode() ?? 0;
  }

  public static bool operator ==(Tag? left, Tag? right) {
    if (left is null) {
      return right is null;
    }

    return left.Equals(right);
  }

  public static bool operator !=(Tag? left, Tag? right) => !(left == right);
}