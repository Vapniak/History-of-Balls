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

  public IEnumerable<Tag> GetAllChildren() {
    var tags = new List<Tag>();
    foreach (var child in GetChildren()) {
      tags.Add(child);
      tags.AddRange(child.GetAllChildren());
    }

    return tags;
  }

  public bool IsChildOf(Tag? parent) =>
    parent != null &&
    FullName != parent.FullName &&
    FullName.StartsWith(parent.FullName);


  public IEnumerable<Tag> GetChildren() {
    return _children;
  }

  public bool IsExact(Tag? other) {
    return other != null && other.FullName == FullName;
  }
  public bool Equals(Tag? other) {
    return other != null && (FullName.StartsWith(other.FullName) || FullName == other.FullName);
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

  public override string ToString() => FullName;

  public static bool operator !=(Tag? left, Tag? right) => !(left == right);
}